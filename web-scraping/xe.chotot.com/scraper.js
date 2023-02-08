const cheerio = require('cheerio');
const UserAgent = require('user-agents');
const userAgent = new UserAgent({ deviceCategory: 'desktop' });
const configs = require('./configs');
const utils = require('../utils');
const database = require('../database');
const fs = require('fs');

const scraperObject = {
    async scraper(browser, categories, pageUrl)
    {
        try 
        {
            if (!utils.validateUrl(pageUrl)) 
            {
                await database.scrapeLogInsert({
                    SiteId: configs.siteId,
                    Path: pageUrl,
                    Message: 'Url không hợp lệ'
                });

                return;
            }

            // Open a new page
            const page = await browser.newPage();

            page.setUserAgent(userAgent.random().toString());

            await page.setRequestInterception(true);

            page.on('request', (request) => {

                if (!['document', 'xhr', 'fetch', 'script'].includes(request.resourceType())) 
                {
                    return request.abort();
                } 

                request.continue();
            });

            console.log(`Truy cập danh sách bài đăng =>\n${pageUrl}\n`);

            await database.scrapeLogInsert({
                SiteId: configs.siteId,
                Path: pageUrl,
                Message: `Truy cập danh sách bài đăng => ${pageUrl}`
            });

            await page.setDefaultNavigationTimeout(0);
            
            await page.goto(pageUrl , {timeout: 60000, waitUntil: 'domcontentloaded'}); //{ waitUntil: 'domcontentloaded'}

            const scrapeCurrentPage = async (pageUrl) =>
            {
                try 
                {
                    console.log(`Thu thập url bài đăng =>\n${pageUrl}\n`);

                    let productUrls = await page.$$eval('.AdItem_wrapperAdItem__S6qPH', card => 
                    {
                        return card.map(el =>
                        {
                            const href = el.querySelector('a.AdItem_adItem__gDDQT').href;

                            let productUrl = href.substring(0, href.indexOf('.htm') + 4);

                            return productUrl;
                        });
                    });

                    //console.log(productUrls);

                    let pagePromise = (productUrl) => new Promise(async(resolve, reject) =>{
                        try 
                        {
                            let newPage = await browser.newPage();

                            newPage.setUserAgent(userAgent.random().toString());

                            await newPage.setRequestInterception(true);

                            newPage.on('request', (request) => {

                                if (!['document', 'xhr', 'fetch', 'script'].includes(request.resourceType())) 
                                {
                                    return request.abort();
                                } 
                
                                request.continue();
                            });

                            console.log(`Truy cập bài đăng =>\n${productUrl}\n`);

                            const [response] = await Promise.all([
                                
                                newPage.waitForResponse(res => res.url().includes(configs.phoneRequestUrl), {timeout: 90000}),
                                
                                newPage.goto(productUrl, {waitUntil: 'domcontentloaded'})
                            ]);

                            const phoneResponse = await response.json();

                            const phoneNumber = phoneResponse.phone;

                            const pageHtml = await newPage.evaluate(() => document.querySelector('*').outerHTML);

                            const $ = cheerio.load(pageHtml);

                            const nextData = $('#__NEXT_DATA__');

                            if(nextData.length > 0)
                            {
                                const nextDataJson = JSON.parse(nextData.text());

                                // try 
                                // {
                                //     fs.writeFileSync('nextData.txt', nextData.text());
                                // } 
                                // catch (err) 
                                // {
                                //     console.error(err);
                                // }

                                if(nextDataJson)
                                {
                                    nextDataJson.phoneNumber = phoneNumber;

                                    await parserData(nextDataJson, pageUrl, productUrl);
                                }
                            }

                            await waitForTimeout(newPage);

                            resolve(true);

                            await pageClose(newPage, productUrl);
                        } 
                        catch (error) 
                        {
                            reject(false);

                            console.error(`pagePromise error => ${error.message}\n stack trace => ${error.stack}\n`);
                        }
                    });

                    for(link in productUrls)
                    {
                        await pagePromise(productUrls[link]);
                    }

                    let currentPage = utils.urlGetParam(pageUrl, 'page');

                    if(currentPage <= 1000)
                    {
                        let nextPageUrl = utils.urlSetParam(pageUrl, 'page', (currentPage + 1));

                        console.log(`Truy cập trang => ${(currentPage + 1)} => danh sách bài đăng =>\n${nextPageUrl}\n`);

                        await page.goto(nextPageUrl, { waitUntil: 'domcontentloaded'});

                        await database.scrapeLogInsert({
                            SiteId: configs.siteId,
                            Path: nextPageUrl,
                            Message: `Truy cập trang => ${(currentPage + 1)}`
                        })

                        return scrapeCurrentPage(nextPageUrl);
                    }

                     //đóng page
                     await pageClose(page, pageUrl);
                } 
                catch (error) 
                {
                    console.error(`scrapeCurrentPage error => ${error.message}\n stack trace => ${error.stack}\n`);
                }
            }

            const parserData = async (data, pageUrl, productUrl) =>
            {
                try 
                {
                    let districtId = 0 , wardId = 0, streetId = 0, productId = 0;

                    if(data.props.initialState && data.props.initialProps)
                    {
                        const [[parentCategoryId, categoryId], actionTypeId, provinceId, customerId] = await Promise.all([
                            parserCategory(data, pageUrl, productUrl),
                            parserActionType(data, pageUrl, productUrl),
                            parserProvince(data, pageUrl, productUrl),
                            parserCustomer(data, pageUrl, productUrl)
                        ]);

                        if(provinceId > 0)
                        {
                            districtId = await parserDistrict(data, provinceId, pageUrl, productUrl);

                            if(districtId > 0)
                            {
                                wardId = await parserWards(data, provinceId, districtId, pageUrl, productUrl);

                                if(wardId > 0)
                                {
                                    streetId = await parserStreet(data, provinceId, districtId, wardId, pageUrl, productUrl);
                                }
                            }

                            productId = await parserProduct(data, categoryId, parentCategoryId, actionTypeId, provinceId, (districtId || 0), (wardId || 0), (streetId || 0), customerId, pageUrl, productUrl);
                        
                            // if(productId > 0)
                            // {
                            //     await parserCategory(data, productId, pageUrl, productUrl);
                            // }
                        }
                    }
                    
                    await waitForTimeout(page);
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserData', error, pageUrl, productUrl);
                }
            }

            const parserCategory = async (data, pageUrl, productUrl) =>
            {
                let parentCategoryId = null, categoryId = null;
                try 
                {
                    if(data.props.initialState.nav.navObj.categoryObj.label.length > 0)
                    {
                        const categoryName = data.props.initialState.nav.navObj.categoryObj.label;

                        const parentCaegoryName = getParentCategory(categoryName);

                        if(parentCaegoryName != null)
                        {
                            const parentCategory = 
                            {
                                SiteId: configs.siteId,
                                Name: parentCaegoryName,
                            }

                            parentCategoryId = await database.categoryInsert(parentCategory);
                        }

                        const category = 
                        {
                            SiteId: configs.siteId,
                            ParentId: parentCategoryId,
                            Name: categoryName,
                        }

                        categoryId = await database.categoryInsert(category);

                        // if(parentId != null && parentId > 0)
                        // {
                        //     const parentProductCategory = 
                        //     {
                        //         SiteId: configs.siteId,
                        //         ProductId: productId,
                        //         CategoryId: parentId
                        //     }

                        //     await database.productCategoryInsert(parentProductCategory);
                        // }

                        // if(categoryId > 0)
                        // {
                        //     const productCategory = 
                        //     {
                        //         SiteId: configs.siteId,
                        //         ProductId: productId,
                        //         CategoryId: categoryId
                        //     }

                        //     await database.productCategoryInsert(productCategory);
                        // }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserCategory', error, pageUrl, productUrl);
                }

                return [parentCategoryId, categoryId];
            }

            const parserActionType = async (data, pageUrl, productUrl) =>
            {
                let actionTypeId = 0;
                try 
                {
                    if(data.props.initialState.adView.adInfo.ad.type_name && data.props.initialState.adView.adInfo.ad.type_name.length > 0)
                    {
                        const actionType = 
                        {
                            SiteId: configs.siteId,
                            Name: data.props.initialState.adView.adInfo.ad.type_name
                        }

                        actionTypeId = await database.actionTypeInsert(actionType);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserActionType', error, pageUrl, productUrl);
                }

                return actionTypeId;
            }

            const parserProvince = async (data, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    if(data.props.initialState.adView.adInfo.ad && data.props.initialState.adView.adInfo.ad.region_name && data.props.initialState.adView.adInfo.ad.region_name.length > 0)
                    {
                        let province = {
                            SiteId: configs.siteId,
                            Name: data.props.initialState.adView.adInfo.ad.region_name
                        }

                        resultVar = await database.provinceInsert(province);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserProvince', error, pageUrl, productUrl);
                }

                return resultVar;
            }

            const parserDistrict = async (data, provinceId, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    if(provinceId > 0 && data.props.initialState.adView.adInfo.ad && data.props.initialState.adView.adInfo.ad.area_name && data.props.initialState.adView.adInfo.ad.area_name.length > 0)
                    {
                        let district = {
                            SiteId: configs.siteId,
                            ProvinceId: provinceId,
                            Name: data.props.initialState.adView.adInfo.ad.area_name.replace('Quận', '').replace('Huyện', '').trim()
                        }

                        resultVar = await database.districtInsert(district);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserDistrict', error, pageUrl, productUrl);
                }

                return resultVar;
            }

            const parserWards = async (data, provinceId, districtId, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    if(provinceId > 0 && districtId > 0 && data.props.initialState.adView.adInfo.ad && data.props.initialState.adView.adInfo.ad.ward_name && data.props.initialState.adView.adInfo.ad.ward_name.length > 0)
                    {
                        let wards = {
                            SiteId: configs.siteId,
                            ProvinceId: provinceId,
                            DistrictId: districtId,
                            Name: data.props.initialState.adView.adInfo.ad.ward_name.replace('Phường', '').replace('Xã', '').trim()
                        }

                        resultVar = await database.wardsInsert(wards);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserWards', error, pageUrl, productUrl);
                }

                return resultVar;
            }

            const parserStreet = async (data, provinceId, districtId, wardId, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    if(provinceId > 0 && districtId > 0 && data.props.initialState.adView.adInfo.ad && data.props.initialState.adView.adInfo.ad.street_name && data.props.initialState.adView.adInfo.ad.street_name.length > 0)
                    {
                        let streetName = data.props.initialState.adView.adInfo.ad && data.props.initialState.adView.adInfo.ad.street_name;
                        const streetNumber = data.props.initialState.adView.adInfo.ad && data.props.initialState.adView.adInfo.ad.street_number || '';

                        if(streetNumber.length > 0)
                        {
                            streetName = `${streetNumber}, ${streetName}`;
                        }

                        let street = {
                            SiteId: configs.siteId,
                            ProvinceId: provinceId,
                            DistrictId: districtId,
                            WardId: wardId,
                            Name: streetName
                        }

                        resultVar = await database.streetInsert(street);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserStreet', error, pageUrl, productUrl);
                }

                return resultVar;
            }

            const parserCustomer = async (data, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    if(data.props.initialState.adView.adInfo.ad.account_name.length > 0 || data.phoneNumber.length > 0)
                    {
                        let customer = {
                            SiteId: configs.siteId,
                            FullName: data.props.initialState.adView.adInfo.ad.account_name,
                            PhoneNumber: data.phoneNumber,
                            Email: null,
                            Avatar: (data.props.initialState.adView.adInfo.ad.avatar || null)
                        }

                        resultVar = await database.customerInsert(customer);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserCustomer', error, pageUrl, productUrl);
                }

                return resultVar;
            }

            const parserProduct = async (data, categoryId, parentCategoryId, actionTypeId, provinceId, districtId, wardId, streetId, customerId, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    if(data.props.initialState.adView.adInfo.ad)
                    {
                        let title = data.props.initialState.adView.adInfo.ad.subject || '';

                        if(title.length > 0)
                        {
                            let productCode = data.props.initialProps.pageProps.adId,
                            productUrl = data.props.initialProps.canonicalUrl,
                            imagePath = data.props.initialState.adView.adInfo.ad.thumbnail_image,
                            breadcrumb = null,
                            address = null;
                            isVideo = (data.props.initialState.adView.adInfo.ad.videos.length > 0 ? 1 : 0),
                            verified = (data.props.initialState.adView.adInfo.ad.protection_entitlement ? 1 : 0),
                            unixTimestamp = data.props.initialState.adView.adInfo.ad.list_time,
                            publishedAt = utils.convertTZ(new Date(unixTimestamp), 'Asia/Ho_Chi_Minh'),
                            expirationAt = null;

                            let product = {
                                SiteId: configs.siteId,
                                CategoryId: categoryId,
                                ParentCategoryId: parentCategoryId,
                                Title: title,
                                ProductUrl: productUrl,
                                ImagePath: imagePath,
                                ProductCode: productCode,
                                ProvinceId: provinceId,
                                DistrictId: districtId,
                                WardId: wardId,
                                StreetId: streetId,
                                CustomerId: customerId,
                                Breadcrumb: breadcrumb,
                                Address: address,
                                Verified: verified,
                                IsVideo: isVideo,
                                ActionTypeId: actionTypeId,
                                PublishedAt: publishedAt,
                                ExpirationAt: expirationAt
                            }
    
                            resultVar = await database.productInsert(product);
                        }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserProduct', error, pageUrl, productUrl);
                }

                return resultVar;
            }

            const getParentCategory = (categoryName) =>
            {
                let parentCaegory = null;
                try 
                {
                    for(key in categories)
                    {
                        //if(key == 'mua-ban-xe')
                        //{
                            const item = categories[key];

                            if(item.subCategories.entities)
                            {
                                for(k in item.subCategories.entities)
                                {
                                    const category = item.subCategories.entities[k];
    
                                    if(category.name == categoryName)
                                    {
                                        if(parentCaegory == null && item.name != categoryName)
                                        {
                                            parentCaegory = item.name;
                                        }
                                    }
                                }
                            }
                        //}
                    }
                } 
                catch (error) 
                {
                    console.log(`getParentCategory error => ${error}\n`);
                }

                return parentCaegory;
            }

            const pageClose = async (page, path) =>
            {
                try 
                {
                    await page.close();

                    console.log(`Đóng page => ${path || ''}\n`);
                } 
                catch (error) 
                {
                    console.error(`pageClose => ${path || ''} error => ${error.message}\n stack trace => ${error.stack}\n`);
                }
            }

            const waitForTimeout = async (page) =>
            {
                const randomNumber = utils.getRandomNumber();

                try 
                {
                    console.log(`Đợi xử lý sau => ${randomNumber/1000} giây...\n`);

                    await page.waitForTimeout(randomNumber);
                } 
                catch (error) 
                {
                    console.error(`waitForTimeout('${randomNumber}') error => ${error.message}\n stack trace => ${error.stack}\n`);
                }
            }

            //
            await waitForTimeout(page);
            
            await scrapeCurrentPage(pageUrl);
        } 
        catch (error) 
        {
            console.error(`scraper error => ${error.message}\n stack trace => ${error.stack}\n`);
        }
    },
    async scraperLog(message, error, pageUrl, productUrl)
    {
        try 
        {
            console.error(`${message} error => ${error.message}\n stack trace => ${error.stack}\n`);

            await database.scrapeLogInsert({
                siteId: configs.siteId,
                Path: pageUrl,
                DetailPath: (productUrl || null),
                Message: `${message} error => ${error}`
            });
        } 
        catch (error) 
        {
            console.error(`scraperLog error => ${error.message}\n stack trace => ${error.stack}\n`);
        }
    }
}

module.exports = scraperObject;
const cheerio = require('cheerio');
const UserAgent = require('user-agents');
const userAgent = new UserAgent({ deviceCategory: 'desktop' });
const configs = require('./configs');
const utils = require('../utils');
const database = require('../database');

const scraperObject = {
    async scraper(browser, pageUrl)
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
            
            await page.goto(pageUrl , {timeout: 60000, waitUntil: 'domcontentloaded'});

            const scrapeCurrentPage = async (pageUrl) =>
            {
                try 
                {
                    console.log(`Thu thập url bài đăng =>\n${pageUrl}\n`);

                    let productUrls = await page.$$eval('.property-list .row-item', card => 
                    {
                        return card.map(el =>
                        {
                            return el.querySelector('a').href;
                        });
                    });

                    console.log(productUrls);

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

                            await newPage.setDefaultNavigationTimeout(0);
            
                            await newPage.goto(productUrl , {timeout: 60000, waitUntil: 'domcontentloaded'});

                            const pageHtml = await newPage.evaluate(() => document.querySelector('*').outerHTML);

                            const $ = cheerio.load(pageHtml);
                            let breadcrumbElement = $('.breadcrumb').first();

                            if(breadcrumbElement.length > 0)
                            {
                                let breadcrumbs = breadcrumbElement.text().trim().replace(/\r/g, '').split(/\n/).filter(function(v){return v!==''});

                                console.log(breadcrumbs)
                            }

                            await parserData($, pageUrl, productUrl);
                                
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

                    // let currentPage = utils.urlGetParam(pageUrl, 'page');

                    // if(currentPage <= 1000)
                    // {
                    //     let nextPageUrl = utils.urlSetParam(pageUrl, 'page', (currentPage + 1));

                    //     console.log(`Truy cập trang => ${(currentPage + 1)} => danh sách bài đăng =>\n${nextPageUrl}\n`);

                    //     await page.goto(nextPageUrl, { waitUntil: 'domcontentloaded'});

                    //     await database.scrapeLogInsert({
                    //         SiteId: configs.siteId,
                    //         Path: nextPageUrl,
                    //         Message: `Truy cập trang => ${(currentPage + 1)}`
                    //     })

                    //     return scrapeCurrentPage(nextPageUrl);
                    // }

                     //đóng page
                     await pageClose(page, pageUrl);
                } 
                catch (error) 
                {
                    console.error(`scrapeCurrentPage error => ${error.message}\n stack trace => ${error.stack}\n`);
                }
            }

            const parserData = async ($, pageUrl, productUrl) =>
            {
                try 
                {
                    let districtId = 0 , wardId = 0, productId = 0;

                    const [actionTypeId, categoryId, provinceId, customerId] = await Promise.all([
                        parserActionType($, pageUrl, productUrl),
                        parserCategory($, pageUrl, productUrl),
                        parserProvince($, pageUrl, productUrl),
                        parserCustomer($, pageUrl, productUrl)
                    ]);

                    if(provinceId > 0 || 1==1)
                    {
                        districtId = await parserDistrict($, provinceId, pageUrl, productUrl);

                        if(districtId > 0 || 1==1)
                        {
                            wardId = await parserWards($, provinceId, districtId, pageUrl, productUrl);
                        }

                        productId = await parserProduct($, actionTypeId, categoryId, provinceId, (districtId || 0), (wardId || 0), customerId, pageUrl, productUrl);
                    }
                    
                    await waitForTimeout(page);
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserData', error, pageUrl, productUrl);
                }
            }

            const parserActionType = async ($, pageUrl, productUrl) =>
            {
                let actionTypeId = 0;
                try 
                {
                    let breadcrumbElement = $('.breadcrumb').first();

                    if(breadcrumbElement.length > 0)
                    {
                        let breadcrumbs = breadcrumbElement.text().trim().replace(/\r/g, '').split(/\n/).filter(function(v){return v!==''});

                        if(breadcrumbs.length > 1)
                        {
                            const actionTypeName = breadcrumbs[1].trim();

                            if(actionTypeName.length > 0)
                            {
                                const actionType = 
                                {
                                    SiteId: configs.siteId,
                                    Name: actionTypeName.replace('Mua bán nhà đất', 'Bán').replace('Cho thuê nhà đất', 'Cho thuê')
                                }
                                console.log(actionType);
                                //actionTypeId = await database.actionTypeInsert(actionType);
                            }
                        }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserActionType', error, pageUrl, productUrl);
                }

                return actionTypeId;
            }

            const parserCategory = async ($, pageUrl, productUrl) =>
            {
                let categoryId = null;
                try 
                {

                    let breadcrumbElement = $('.breadcrumb').first();

                    if(breadcrumbElement.length > 0)
                    {
                        let breadcrumbs = breadcrumbElement.text().trim().replace(/\r/g, '').split(/\n/).filter(function(v){return v!==''});

                        if(breadcrumbs.length > 2)
                        {
                            const categoryName = breadcrumbs[2].trim();

                            if(categoryName.length > 0)
                            {
                                const category = 
                                {
                                    SiteId: configs.siteId,
                                    Name: utils.ucFirst(categoryName.replace('Mua bán', '').replace('Cho thuê', '').trim())
                                }
                                console.log(category);
                                //categoryId = await database.categoryInsert(category);
                            }
                        }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserCategory', error, pageUrl, productUrl);
                }

                return categoryId;
            }

            const parserProvince = async ($, pageUrl, productUrl) =>
            {
                let provinceId = 0;
                try 
                {
                    let breadcrumbElement = $('.reales-location .col-left .infor').first();

                    if(breadcrumbElement.length > 0)
                    {
                        let breadcrumbs = breadcrumbElement.text().replace('Vị trí:', '').trim().split('-');

                        if(breadcrumbs.length > 0)
                        {
                            const provinceName = breadcrumbs[breadcrumbs.length -1].trim();

                            if(provinceName.length > 0)
                            {
                                const province = 
                                {
                                    SiteId: configs.siteId,
                                    Name: provinceName
                                }
                                console.log(province);
                                //provinceId = await database.provinceInsert(province);
                            }
                        }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserProvince', error, pageUrl, productUrl);
                }

                return provinceId;
            }

            const parserDistrict = async ($, provinceId, pageUrl, productUrl) =>
            {
                let districtId = 0;
                try 
                {
                    let breadcrumbElement = $('.breadcrumb').first();

                    if(breadcrumbElement.length > 0)
                    {
                        let breadcrumbs = breadcrumbElement.text().trim().replace(/\r/g, '').split(/\n/).filter(function(v){return v!==''});

                        if((provinceId > 0 || 1==1) && breadcrumbs.length > 4)
                        {
                            const districtName = breadcrumbs[4].trim();

                            if(districtName.length > 0)
                            {
                                const district = 
                                {
                                    SiteId: configs.siteId,
                                    ProvinceId: provinceId,
                                    Name: districtName.replace('Quận', '').replace('Huyện', '').trim()
                                }
                                console.log(district);
                                //districtId = await database.districtInsert(district);
                            }
                        }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserDistrict', error, pageUrl, productUrl);
                }

                return districtId;
            }

            const parserWards = async ($, provinceId, districtId, pageUrl, productUrl) =>
            {
                let wardsId = 0;
                try 
                {
                    let breadcrumbElement = $('.breadcrumb').first();

                    if(breadcrumbElement.length > 0)
                    {
                        let breadcrumbs = breadcrumbElement.text().trim().replace(/\r/g, '').split(/\n/).filter(function(v){return v!==''});

                        if((provinceId > 0 && districtId > 0 || 1==1) && breadcrumbs.length > 5)
                        {
                            const wardsName = breadcrumbs[5].trim();

                            if(wardsName.length > 0)
                            {
                                const wards = 
                                {
                                    SiteId: configs.siteId,
                                    ProvinceId: provinceId,
                                    DistrictId: districtId,
                                    Name: wardsName.replace('Phường', '').replace('Xã', '').replace('Thị trấn', '').replace('thị trấn', '').trim()
                                }
                                console.log(wards);
                                //wardsId = await database.wardsInsert(wards);
                            }
                        }
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserWards', error, pageUrl, productUrl);
                }

                return wardsId;
            }

            const parserCustomer = async ($, pageUrl, productUrl) =>
            {
                let customerId = 0;
                try 
                {
                    const contactInfoElement = $('.block-contact-infor').first();

                    if(contactInfoElement.length > 0)
                    {
                        let fullName = '', phoneNumber = '', email = null, avatar = null;

                        const contactNameElement = contactInfoElement.find('.profile-name').first();

                        if(contactNameElement.length > 0)
                        {
                            fullName = contactNameElement.text().trim();
                        }

                        const phoneElement = contactInfoElement.find('.detailTelProfile').first();

                        if(phoneElement.length > 0)
                        {
                            phoneNumber = (phoneElement.attr('onclick') || '').replace('showfullphone(this,\'', '').replace('\')', '').trim();
                        }

                        const emailElement = contactInfoElement.find('.profile-email a').first();
                        
                        if(emailElement.length > 0)
                        {
                            email = emailElement.text().trim();
                        }

                        const avatarElement = contactInfoElement.find('.profile-avatar img').first();
                        
                        if(avatarElement.length > 0)
                        {
                            avatar = (avatarElement.attr('src') || '').trim();
                        }

                        let customer = {
                            SiteId: configs.siteId,
                            FullName: fullName,
                            PhoneNumber: phoneNumber,
                            Email: email,
                            Avatar: avatar
                        }
                        console.log(customer);
                        //customerId = await database.customerInsert(customer);
                    }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserCustomer', error, pageUrl, productUrl);
                }

                return customerId;
            }

            const parserProduct = async ($, actionTypeId, categoryId, provinceId, districtId, wardId, customerId, pageUrl, productUrl) =>
            {
                let resultVar = 0;
                try 
                {
                    const productDetailWebElement = $('.detail-property').first();
                    
                    if(productDetailWebElement.length > 0)
                    {
                        let title = '', breadcrumb = '', address = '', productCode = 0, 
                        publishedAt = null, expirationAt = null, verified = 0, isVideo = 0;


                        //tin đã xác thực
                        const iconVerifiedElement = productDetailWebElement.find('.reals-cafeland-xacnhan').first();

                        if(iconVerifiedElement.length > 0)
                        {
                            verified = 1;
                        }

                        const breadcrumbElement = $('.breadcrumb').first();

                        if(breadcrumbElement.length > 0)
                        {
                            let breadcrumbs = breadcrumbElement.text().trim().replace(/\r/g, '').split(/\n/).filter(function(v){return v!==''});

                            if(breadcrumbs.length > 0)
                            {
                                breadcrumb = breadcrumbs.join('/');
                            }
                        }

                        const addressElement = productDetailWebElement.find('.reales-location .col-left .infor').first();

                        if(addressElement.length > 0)
                        {
                            address = addressElement.text().replace('Vị trí:', '').replace(/\n/g, ' ').trim();
                        }

                        const productTitleElement = productDetailWebElement.find('.head-title').first();

                        if(productTitleElement.length > 0)
                        {
                            title = productTitleElement.text().trim();
                        }

                        //mã tin - ngày đăng
                        const productCodeElement = $('.reales-location .col-right .infor').first();

                        if(productCodeElement.length > 0)
                        {
                            try 
                            {
                                const productCodes = productCodeElement.text().split('/');

                                if(productCodes.length > 0)
                                {
                                    const productCodeClean = productCodes[0].replace(/[^0-9]/gm,'').trim();

                                    if(productCodeClean.length > 4)
                                    {
                                        productCode = parseInt(productCodeClean.substring(4));
                                    }

                                    if(productCodes.length > 1)
                                    {
                                        const publishedAtSplit = productCodes[1].replace('Cập nhật:', '').trim().split('-');

                                        if(publishedAtSplit.length == 3)
                                        {
                                            const [ publishedAtError, publishedAtData] = utils.dateToISOString(publishedAtSplit[0] , publishedAtSplit[1], publishedAtSplit[2]);
                                    
                                            if(publishedAtError)
                                            {
                                                await scraperObject.scraperLog(`Bài đăng => ${title} => PublishedAt`, publishedAtError, pageUrl, productUrl);
                                            }
                                            else
                                            {
                                                publishedAt = publishedAtData;
                                            }
                                        }
                                    }
                                    
                                }
                            } 
                            catch (error) 
                            {
                                await scraperObject.scraperLog(`Bài đăng ${title} => ProductCode`, error, pageUrl, productUrl);
                            }
                        }

                        let product = {
                            SiteId: configs.siteId,
                            Title: title,
                            ProductUrl: productUrl,
                            ProductCode: productCode,
                            ProvinceId: provinceId,
                            DistrictId: districtId,
                            WardId: wardId,
                            //StreetId: streetId,
                            //ProjectId: projectId,
                            CustomerId: customerId,
                            Breadcrumb: breadcrumb,
                            Address: address,
                            Verified: verified,
                            IsVideo: isVideo,
                            //ActionTypeId: actionTypeId,
                            //LandTypeId: landTypeId,
                            PublishedAt: publishedAt,
                            ExpirationAt: expirationAt
                        }

                        console.log(product);

                        //resultVar = await database.productInsert(product);
                    }
                    ///
                    // if(data.props.initialState.adView.adInfo.ad)
                    // {
                    //     let title = data.props.initialState.adView.adInfo.ad.subject || '';

                    //     if(title.length > 0)
                    //     {
                    //         let productCode = data.props.initialProps.pageProps.adId,
                    //         productUrl = data.props.initialProps.canonicalUrl,
                    //         imagePath = data.props.initialState.adView.adInfo.ad.thumbnail_image,
                    //         breadcrumb = null,
                    //         address = null;
                    //         isVideo = (data.props.initialState.adView.adInfo.ad.videos.length > 0 ? 1 : 0),
                    //         verified = (data.props.initialState.adView.adInfo.ad.protection_entitlement ? 1 : 0),
                    //         unixTimestamp = data.props.initialState.adView.adInfo.ad.list_time,
                    //         publishedAt = utils.convertTZ(new Date(unixTimestamp), 'Asia/Ho_Chi_Minh'),
                    //         expirationAt = null;

                    //         let product = {
                    //             SiteId: configs.siteId,
                    //             CategoryId: categoryId,
                    //             ParentCategoryId: parentCategoryId,
                    //             Title: title,
                    //             ProductUrl: productUrl,
                    //             ImagePath: imagePath,
                    //             ProductCode: productCode,
                    //             ProvinceId: provinceId,
                    //             DistrictId: districtId,
                    //             WardId: wardId,
                    //             StreetId: streetId,
                    //             CustomerId: customerId,
                    //             Breadcrumb: breadcrumb,
                    //             Address: address,
                    //             Verified: verified,
                    //             IsVideo: isVideo,
                    //             ActionTypeId: actionTypeId,
                    //             PublishedAt: publishedAt,
                    //             ExpirationAt: expirationAt
                    //         }
    
                    //         resultVar = await database.productInsert(product);
                    //     }
                    // }
                } 
                catch (error) 
                {
                    await scraperObject.scraperLog('parserProduct', error, pageUrl, productUrl);
                }

                return resultVar;
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
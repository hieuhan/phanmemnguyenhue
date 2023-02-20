const configs = require('./configs');

function validateUrl(url)
{
    let resultVar = false;

    const regexUrl = new RegExp(/(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})/gi);

    if (!url.match(regexUrl)) 
    {
        console.error(`Url => ${url} không hợp lệ\n`);
    }
    else
    {
        resultVar = true;
    }

    return resultVar;
}

function dateToISOString(day, month, year)
{
    try 
    {
        return [undefined, new Date(`${year.trim()}-${month.trim()}-${day.trim()}`).toISOString()];
    } 
    catch (error) 
    {
        return [error, undefined]
    }
}

function convertTZ(date, tzString) 
{
    return new Date((typeof date === 'string' ? new Date(date) : date).toLocaleString('en-US', {timeZone: tzString}));   
}

function getRandomNumber()
{
    let resultVar = 0;
    try 
    {
        resultVar = Math.floor(Math.random() * (configs.randomMax - configs.randomMin + 1)) + configs.randomMin;
    } 
    catch (error) 
    {
        console.error(`getRandomNumber error => ${error}\n`);
    }
    
    return resultVar;
}

function getProductUrl(websiteDomain, path)
{
    let resultVar = '';
    try 
    {
        if(path)
        {
            resultVar = path.trim();
            
            if(resultVar.length > 0)
            {
                if(resultVar.indexOf('://') <= 0)
                {   
                    while(resultVar.startsWith('/'))
                    {
                        resultVar = resultVar.substring(1);
                    }
    
                    resultVar = `${websiteDomain}${resultVar}`;
                }
            }
        }
    } 
    catch (error) 
    {
        console.error(`getProductUrl error => ${error}\n`);
    }

    return resultVar;
}

const autoScroll = async (page) =>
{
    await page.evaluate(async () => {
        await new Promise((resolve) => {
            var totalHeight = 0;
            var distance = 100;
            var timer = setInterval(() => {
                var scrollHeight = document.body.scrollHeight;
                window.scrollBy(0, distance);
                totalHeight += distance;

                if(totalHeight >= scrollHeight - window.innerHeight){
                    clearInterval(timer);
                    resolve();
                }
            }, 100);
        });
    });
}

const _isVisible = async(page, elementHandle) => await page.evaluate((el) => {
    
    if (!el || el.offsetParent === null) 
    {
        return false;
    }
  
    const style = window.getComputedStyle(el);

    return style && style.display !== 'none' && style.visibility !== 'hidden' && style.opacity !== '0';

}, elementHandle)

const waitForVisible = async(page, selector, timeout = 25) => {

    const startTime = new Date();

    try 
    {
        //await page.waitForSelector(selector , {visible: true});
        await page.waitForFunction(`document.querySelector('${selector}') && document.querySelector('${selector}').clientHeight != 0`);
        
        // Keep looking for the first visible element matching selector until timeout
        while (true) 
        {
            const els = await page.$$(selector);

            for(const el of els) 
            {
                if (await _isVisible(page, el)) 
                {
                    console.log(`PASS Check visible : ${selector}`);
                    return el;
                }
            }

            if (new Date() - startTime > timeout) 
            {
                throw new Error(`Timeout after ${timeout}ms`);
            }

            page.waitFor(50);
        }
    } 
    catch (e) 
    {
        console.log(`FAIL Check visible : ${selector} - ${e}`);

        return false;
    }               
}

function urlGetParam(pageUrl, param)
{
    let resultVar = '';

    try 
    {
        let currentUrl = new URL(pageUrl);

        resultVar = parseInt(currentUrl.searchParams.get(param) || 1);
    } 
    catch (error) 
    {
        console.error(`urlGetParam error => ${error}\n`);
    }

    return resultVar;
}

function urlSetParam(pageUrl, param, value)
{
    let resultVar = '';

    try 
    {
        let currentUrl = new URL(pageUrl);

        currentUrl.searchParams.set(param, value);

        resultVar = currentUrl.href; 
    } 
    catch (error) 
    {
        console.error(`urlSetParam error => ${error}\n`);
    }

    return resultVar;
}

function urlGetCurrentPage(url)
{
    let currentPage = 1;
    try 
    {
        currentPage = url.substring(url.lastIndexOf('/') + 1).replace('p','');
    } 
    catch (error) 
    {
        console.error(`getCurrentPageUrl('${url}') error => ${error}\n`);
    }

    return currentPage;
}

function ucFirst(string){
    return string.charAt(0).toUpperCase() + string.slice(1);
}

module.exports = {
    validateUrl: validateUrl,
    dateToISOString: dateToISOString,
    convertTZ: convertTZ,
    getRandomNumber: getRandomNumber,
    getProductUrl: getProductUrl,
    autoScroll: autoScroll,
    waitForVisible: waitForVisible,
    urlGetParam: urlGetParam,
    urlSetParam: urlSetParam,
    urlGetCurrentPage: urlGetCurrentPage,
    ucFirst: ucFirst
}
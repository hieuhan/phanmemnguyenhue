const browserObject = require('../browser');
const scraperObject = require('./scraper');
const path = require('path');
const fs = require('fs');

(async () => {
    
    let browser = await browserObject.startBrowser();

    if(browser != null)
    {
        await Promise.all([
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-can-ho-chung-cu/')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
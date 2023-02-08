const browserObject = require('../browser');
const scraperObject = require('./scraper');
const path = require('path');
const fs = require('fs');

(async () => {
    
    let browser = await browserObject.startBrowser();

    if(browser != null)
    {
        let categories = [];
        let rawdata = fs.readFileSync(path.resolve(__dirname, '../categories.txt'));
        categories = JSON.parse(rawdata);

        await Promise.all([
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-xe'),
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-xe-may'),
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-xe-tai-xe-ben'),
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-xe-dien'),
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-xe-dap'),
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-phuong-tien-khac'),
            scraperObject.scraper(browser, categories, 'https://xe.chotot.com/mua-ban-phu-tung-xe')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
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
            scraperObject.scraper(browser, categories, 'https://www.chotot.com/mua-ban-do-dien-tu'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-do-gia-dung-noi-that-cay-canh'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-giai-tri-the-thao-so-thich'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-do-dung-me-va-be'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/dich-vu-du-lich'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-thu-cung'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-do-an-thuc-pham-va-cac-loai-khac'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-tu-lanh-may-lanh-may-giat'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-thoi-trang-do-dung-ca-nhan'),
            scraperObject.scraper(browser, categories,'https://www.chotot.com/mua-ban-do-dung-van-phong-cong-nong-nghiep'),
            scraperObject.scraper(browser, categories, 'https://www.chotot.com/mua-ban?giveaway=true')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
const browserObject = require('../browser');
const scraperObject = require('./scraper-db');
const path = require('path');

(async () => {
    
    let browser = await browserObject.startBrowser();

    if(browser != null)
    {
        await Promise.all([
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-nha-pho/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-nha-rieng/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-nha-biet-thu-lien-ke/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-can-ho-chung-cu/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-nha-hang-khach-san/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/ban-kho-nha-xuong/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/dat-du-an-quy-hoach/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/dat-nong-lam-nghiep/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/nha-dat-ban/dat-nen-dat-o-dat-tho-cu/page-1'),

            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/nha-pho/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/nha-rieng/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/biet-thu/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/can-ho-chung-cu/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/van-phong/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/mat-bang/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/nha-hang-khach-san/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/nha-kho-xuong/page-1'),
            scraperObject.scraper(browser, 'https://nhadat.cafeland.vn/cho-thue/phong-tro/page-1')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
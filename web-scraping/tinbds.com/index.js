const browserObject = require('../browser');
const scraperObject = require('./scraper-db');

(async () => {
    
    let browser = await browserObject.startBrowser();

    if(browser != null)
    {
        await Promise.all([
            scraperObject.scraper(browser, 'https://tinbds.com/nha-dat-ban/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-can-ho-chung-cu/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-nha-rieng/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-nha-biet-thu-lien-ke/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-nha-mat-pho/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-dat-nen-du-an/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-dat/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-trang-trai-khu-nghi-duong/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-kho-nha-xuong/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/ban-loai-bat-dong-san-khac/p-1'),

            scraperObject.scraper(browser, 'https://tinbds.com/nha-dat-cho-thue/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-can-ho-chung-cu/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-nha-rieng/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-nha-mat-pho/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-nha-tro-phong-tro/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-van-phong/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-cua-hang-ki-ot/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-kho-nha-xuong-dat/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/cho-thue-loai-bat-dong-san-khac/p-1'),

            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-can-thue/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-can-ho-chung-cu/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-nha-rieng/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-nha-mat-pho/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-nha-tro-phong-tro/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-van-phong/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-cua-hang-ki-ot/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-kho-nha-xuong-dat/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-thue-loai-bat-dong-san-khac/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-can-ho-chung-cu/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-nha-rieng/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-nha-biet-thu-lien-ke/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-nha-mat-pho/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-dat-nen-du-an/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-dat/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-trang-trai-khu-nghi-duong/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-kho-nha-xuong/p-1'),
            scraperObject.scraper(browser, 'https://tinbds.com/can-mua-loai-bat-dong-san-khac/p-1')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
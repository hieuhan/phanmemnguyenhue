const browserObject = require('../browser');
const scraperObject = require('./scraper');

(async () => {
    
    let browser = await browserObject.startBrowser();

    if(browser != null)
    {
        await Promise.all([
            //scraperObject.scraper(browser,'https://www.nhatot.com/mua-ban-bat-dong-san'),
            scraperObject.scraper(browser,'https://www.nhatot.com/mua-ban-can-ho-chung-cu'),
            scraperObject.scraper(browser,'https://www.nhatot.com/mua-ban-nha-dat'),

            scraperObject.scraper(browser,'https://www.nhatot.com/mua-ban-dat'),
            scraperObject.scraper(browser,'https://www.nhatot.com/sang-nhuong-van-phong-mat-bang-kinh-doanh'),

            //scraperObject.scraper(browser,'https://www.nhatot.com/thue-bat-dong-san'),
            
            scraperObject.scraper(browser,'https://www.nhatot.com/thue-can-ho-chung-cu'),
            scraperObject.scraper(browser,'https://www.nhatot.com/thue-nha-dat'),
            scraperObject.scraper(browser,'https://www.nhatot.com/thue-dat'),
            // scraperObject.scraper(browser,'https://www.nhatot.com/thue-van-phong-mat-bang-kinh-doanh'),
            // scraperObject.scraper(browser,'https://www.nhatot.com/thue-phong-tro')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
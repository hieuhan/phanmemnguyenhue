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
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-ban-hang-sdjt2'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-nhan-vien-phuc-vu-sdjt27'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-giao-hang-sdjt24'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tap-vu-sdjt30'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-nhan-vien-pha-che-sdjt36'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-phu-bep-sdjt33'),

            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-nhan-vien-kinh-doanh-sdjt15'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-cong-nhan-nha-may-sdjt17'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-nhan-vien-kho-sdjt29'),

            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-bao-ve-sdjt7'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-cham-soc-khach-hang-sdjt6'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tai-xe-sdjt3'),

            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-nhan-vien-van-phong-sdjt31'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-ke-toan-sdjt47'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-co-khi-sdjt48'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-telesales-sdjt28'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-giup-viec-sdjt35'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-xay-dung-sdjt44'),

            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-dien-sdjt42'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-thu-ngan-sdjt32'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-le-tan-sdjt41'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-dau-bep-sdjt46'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-cong-nhan-may-sdjt25'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-moc-sdjt23'),


            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-moc-sdjt23'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-sua-xe-sdjt38'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-cat-toc-sdjt34'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-pg-sdjt45'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-lam-nail-sdjt39'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-y-te-sdjt40'),

            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-thu-ky-sdjt43'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-tho-may-tai-nha-sdjt26'),
            scraperObject.scraper(browser, categories, 'https://www.vieclamtot.com/viec-lam-cong-viec-khac-sdjt18')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
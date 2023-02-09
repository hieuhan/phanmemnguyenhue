const browserObject = require('../browser');
const scraperObject = require('./scraper');

(async () => {
    
    let browser = await browserObject.startBrowser();

    if(browser != null)
    {
        await Promise.all([


            //OK
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-ban-binh-duong/p161'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-ban-da-nang/p161'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-ban-khanh-hoa/p161'),

            //OK
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-ban-dong-nai/p1'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-ban-hai-phong/p1'),

            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-cho-thue-ca-mau/p1'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-cho-thue-can-tho/p1'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-cho-thue-cao-bang/p1'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-cho-thue-dak-lak/p1'),
            // scraperObject.scraper(browser,'https://batdongsan.com.vn/nha-dat-cho-thue-dak-nong/p1'),

            //OK
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-dong-thap/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-gia-lai/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ha-giang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ha-nam/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ha-tinh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-hai-duong/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-hau-giang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-hoa-binh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-hung-yen/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-kien-giang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-kon-tum/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-lam-dong/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-lang-son/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-lao-cai/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-long-an/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-nam-dinh/p1'),


            //
            scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-ban/p1'),
            scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue/p1'),

            //OK
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ha-noi/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-tp-hcm/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-binh-duong/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-da-nang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-khanh-hoa/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-dong-nai/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-hai-phong/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ba-ria-vung-tau/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-an-giang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-bac-giang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-bac-lieu/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-bac-ninh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ben-tre/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-binh-dinh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-binh-phuoc/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-binh-thuan/p1'),

            //OK
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-nghe-an/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ninh-binh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-ninh-thuan/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-phu-tho/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-phu-yen/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-quang-binh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-quang-nam/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-quang-ngai/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-quang-ninh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-quang-tri/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-soc-trang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-son-la/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-tay-ninh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-thai-binh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-thai-nguyen/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-thanh-hoa/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-thua-thien-hue/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-tien-giang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-tra-vinh/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-tuyen-quang/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-vinh-long/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-vinh-phuc/p1'),
            // scraperObject.scraper(browser, 'https://batdongsan.com.vn/nha-dat-cho-thue-yen-bai/p1')
        ]);

        console.log('Đóng trình duyệt...');

        await browser.close();
    }

})().catch(error => {
    console.error(`Không thể tạo phiên bản trình duyệt => ${error}\n`);
});
const path = require('path');
require('dotenv').config({ path: path.resolve(__dirname, '.env') });

module.exports =
{
    siteId: process.env.SITE_ID,
    websiteDomain: process.env.WEBSITE_DOMAIN,
    phoneRequestUrl: process.env.PHONE_REQUEST_URL,
    randomMin: parseInt(process.env.RANDOM_MIN),
    randomMax: parseInt(process.env.RANDOM_MAX)
}
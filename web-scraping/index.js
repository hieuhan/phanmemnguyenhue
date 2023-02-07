const axios = require('axios');
const UserAgent = require('user-agents');
const userAgent = new UserAgent({ deviceCategory: 'desktop' });
const fs = require('fs');

let data = [];

axios.get('https://gateway.chotot.com/v1/public/web-proxy-api/loadC2CCategories', {
                headers: { 'User-Agent' : userAgent.random().toString() }
              }).then((response) => {
                //res.header({'key':'value'})     //set response header, if necessary
                data = response.data.allCategoryFollowId.entities;
                try 
                {
                    fs.writeFileSync('categories.txt', JSON.stringify(data));
                } catch (err) {
                    console.error(err);
                }
            });
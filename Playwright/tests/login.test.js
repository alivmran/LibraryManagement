process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
const { test, expect } = require('@playwright/test');

test('JWT Login Test', async ({ request }) => {
  const loginResponse = await request.post('https://localhost:7175/api/users/login', {
    data: {
      email: 'syed.ali.imran2005@gmail.com',
      password: '03022344211'
    }
  });

  const text = await loginResponse.text();
  console.log("Raw Response:", text);

  const body = JSON.parse(text);
  console.log("Parsed Body:", body);

  expect(body.token).toBeTruthy(); // Verify token was returned
});

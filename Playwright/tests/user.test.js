process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
const { test, expect } = require('@playwright/test');

let token = '';
let createdUserId;

test.beforeAll(async ({ request }) => {
  const loginResponse = await request.post('https://localhost:7175/api/users/login', {
    data: {
      email: 'syed.ali.imran2005@gmail.com',
      password: '03022344211'
    }
  });

  const body = await loginResponse.json();
  token = body.token;
});

// POST new user
test('POST new user', async ({ request }) => {
  const res = await request.post('https://localhost:7175/api/users', {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      username: `alice${Date.now()}`,
      email: `alice${Date.now()}@example.com`,
      passwordHash: 'P@ssw0rd!'
    }
  });

  const body = await res.json();
  console.log('POST /users response:', body);
  createdUserId = body.id;
  expect(res.status()).toBe(201);
});

// PUT update user
test('PUT update user', async ({ request }) => {
  const res = await request.put(`https://localhost:7175/api/users/${createdUserId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      id: createdUserId,
      username: 'alice2',
      email: 'alice2@example.com',
      passwordHash: 'NewP@ss!'
    }
  });

  console.log('PUT /users response:', await res.text());
  expect(res.ok()).toBeTruthy();
});

// DELETE user
test('DELETE user', async ({ request }) => {
  const res = await request.delete(`https://localhost:7175/api/users/${createdUserId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  expect(res.status()).toBe(204);
});

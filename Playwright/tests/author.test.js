process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
const { test, expect } = require('@playwright/test');

let token = '';
let createdAuthorId;

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

test('GET authors', async ({ request }) => {
  const res = await request.get('https://localhost:7183/api/authors', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  expect(res.ok()).toBeTruthy();
});

test('POST author', async ({ request }) => {
  const res = await request.post('https://localhost:7183/api/authors', {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      name: 'J. K. Rowling'
    }
  });

  const body = await res.json();
  createdAuthorId = body.id;
  console.log('POST /authors response:', body);
  expect(res.status()).toBe(201);
});

test('PUT author', async ({ request }) => {
  const res = await request.put(`https://localhost:7183/api/authors/${createdAuthorId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      id: createdAuthorId,
      name: 'Joanne Rowling'
    }
  });

  expect(res.ok()).toBeTruthy();
});

test('DELETE author', async ({ request }) => {
  const res = await request.delete(`https://localhost:7183/api/authors/${createdAuthorId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  expect(res.status()).toBe(204);
});

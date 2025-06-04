process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
const { test, expect } = require('@playwright/test');

let token = '';
let createdBookId;
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

  // Create author to assign to book
  const authorRes = await request.post('https://localhost:7183/api/authors', {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      name: 'Temporary Book Author'
    }
  });

  const authorBody = await authorRes.json();
  createdAuthorId = authorBody.id;
});

test('GET books', async ({ request }) => {
  const res = await request.get('https://localhost:7265/api/books', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  expect(res.ok()).toBeTruthy();
});

test('POST book', async ({ request }) => {
  const res = await request.post('https://localhost:7265/api/books', {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      title: "HP Philosopher’s Stone",
      isbn: "9780747532699",
      authorId: createdAuthorId
    }
  });

  const body = await res.json();
  createdBookId = body.id;
  console.log('POST /books response:', body);
  expect(res.status()).toBe(201);
});

test('PUT book', async ({ request }) => {
  const res = await request.put(`https://localhost:7265/api/books/${createdBookId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    },
    data: {
      id: createdBookId,
      title: "HP Philosopher’s Stone - Updated",
      isbn: "9780747532699",
      authorId: createdAuthorId
    }
  });

  expect(res.ok()).toBeTruthy();
});

test('DELETE book', async ({ request }) => {
  const res = await request.delete(`https://localhost:7265/api/books/${createdBookId}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  expect(res.status()).toBe(204);
});

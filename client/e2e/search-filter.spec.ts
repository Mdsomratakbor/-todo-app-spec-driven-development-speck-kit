import { test, expect } from '@playwright/test';

test.describe('Search and Filter', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('should filter todos by status dropdown', async ({ page }) => {
    const filterBar = page.locator('app-filter-bar');
    await filterBar.locator('mat-select').first().click();
    await page.getByRole('option', { name: 'Pending' }).click();
    await expect(page).toHaveURL(/statusId=/);
  });

  test('should filter todos by search text', async ({ page }) => {
    const searchInput = page.getByPlaceholder(/search/i);
    await searchInput.fill('groceries');
    await expect(page).toHaveURL(/search=/);
  });

  test('should clear all filters and reload full list', async ({ page }) => {
    const searchInput = page.getByPlaceholder(/search/i);
    await searchInput.fill('groceries');
    await page.getByRole('button', { name: /clear/i }).click();
    await expect(searchInput).toHaveValue('');
  });
});

import { test, expect } from '@playwright/test';

test.describe('Category Management', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/categories');
  });

  test('should create a new category', async ({ page }) => {
    await page.getByRole('button', { name: 'Add Category' }).click();
    await page.getByPlaceholder('Category name').fill('Work');
    await page.getByRole('button', { name: 'Save' }).click();
    await expect(page.getByText('Category created successfully!')).toBeVisible();
  });

  test('should display list of categories', async ({ page }) => {
    await expect(page.locator('app-category-card').first()).toBeVisible();
  });

  test('should delete a category and reassign todos', async ({ page }) => {
    const deleteButton = page.locator('app-category-card').first().getByRole('button', { name: 'Delete' });
    await deleteButton.click();
    await page.getByRole('button', { name: 'Delete' }).first().click();
    await expect(page.getByText(/Category deleted/)).toBeVisible();
  });

  test('should navigate to todos page and back to categories', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('app-todo-list')).toBeVisible();
    await page.goto('/categories');
    await expect(page.locator('app-category-list')).toBeVisible();
  });
});

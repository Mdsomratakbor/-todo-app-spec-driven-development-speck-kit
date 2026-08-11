import { test, expect } from '@playwright/test';

test.describe('Todo CRUD', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('should create a new todo', async ({ page }) => {
    await page.getByRole('button', { name: 'Add Todo' }).click();
    await page.getByPlaceholder('Enter todo title').fill('Buy groceries');
    await page.getByRole('button', { name: 'Create' }).click();
    await expect(page.getByText('Todo created successfully!')).toBeVisible();
  });

  test('should display list of todos', async ({ page }) => {
    const todoCards = page.locator('app-todo-card');
    await expect(todoCards.first()).toBeVisible();
  });

  test('should edit a todo', async ({ page }) => {
    const editButton = page.locator('app-todo-card').first().getByRole('button', { name: 'Edit' });
    await editButton.click();
    const titleInput = page.getByPlaceholder('Enter todo title');
    await titleInput.fill('Updated title');
    await page.getByRole('button', { name: 'Update' }).click();
    await expect(page.getByText('Todo updated successfully!')).toBeVisible();
  });

  test('should delete a todo and verify it disappears', async ({ page }) => {
    const initialCount = await page.locator('app-todo-card').count();
    const deleteButton = page.locator('app-todo-card').first().getByRole('button', { name: 'Delete' });
    await deleteButton.click();
    await page.getByRole('button', { name: 'Delete' }).first().click();
    await expect(page.getByText('Todo deleted successfully!')).toBeVisible();
    const newCount = await page.locator('app-todo-card').count();
    expect(newCount).toBeLessThan(initialCount);
  });

  test('should validate empty title field', async ({ page }) => {
    await page.getByRole('button', { name: 'Add Todo' }).click();
    await page.getByRole('button', { name: 'Create' }).click();
    await expect(page.getByText('Title is required.')).toBeVisible();
  });
});

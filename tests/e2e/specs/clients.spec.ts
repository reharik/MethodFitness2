import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { expectSaved, openRoute, setField, settle, stamp, submitAndRead, usDate, watchForTrouble } from '../support/app';

test.describe('clients', () => {
  test('add, find, edit and delete a client', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    const lastName = `E2ETest${stamp()}`;
    await login(page);

    // add
    await openRoute(page, 'clientlist');
    await page.locator('#clientlist #new').click();
    await expect(page.locator('#client [name="FirstName"]')).toBeVisible();
    await setField(page, '#client', 'FirstName', 'Auto');
    await setField(page, '#client', 'LastName', lastName);
    await setField(page, '#client', 'Email', `${lastName.toLowerCase()}@example.invalid`);
    await setField(page, '#client', 'MobilePhone', '512-555-0100');
    await setField(page, '#client', 'StartDate', usDate());
    expectSaved(await submitAndRead(page, () => page.locator('#client #save').click()));

    // find it in the list
    await expect(page.locator('#clientlist')).toBeVisible();
    const search = page.locator('#clientlist input.search');
    await search.fill(lastName);
    await search.press('Enter');
    await settle(page, 800);
    const row = page.locator('#gridContainer tr.jqgrow', { hasText: lastName });
    await expect(row).toHaveCount(1);

    // edit
    await row.locator('a').first().click();
    await expect(page.locator('#client [name="LastName"]')).toHaveValue(lastName);
    await setField(page, '#client', 'City', 'Austin');
    expectSaved(await submitAndRead(page, () => page.locator('#client #save').click()));

    // the edit stuck
    await expect(page.locator('#clientlist')).toBeVisible();
    await search.fill(lastName);
    await search.press('Enter');
    await settle(page, 800);
    await row.locator('a').first().click();
    await expect(page.locator('#client [name="City"]')).toHaveValue('Austin');
    await page.locator('#client #cancel').click();

    // delete
    await expect(page.locator('#clientlist')).toBeVisible();
    await search.fill(lastName);
    await search.press('Enter');
    await settle(page, 800);
    await row.locator('input.cbox').check();
    page.once('dialog', (d) => d.accept());
    expectSaved(await submitAndRead(page, () => page.locator('#clientlist button.delete').click(), 'GET', 'Delete'));
    await settle(page, 800);
    await expect(row).toHaveCount(0);

    expect(problems, problems.join('\n')).toEqual([]);
  });
});

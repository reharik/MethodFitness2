import { test, expect } from '@playwright/test';
import { login } from '../support/session';
import { acceptDialogs, expectSaved, listRows, openRoute, searchList, setField, stamp, submitAndRead, usDate, watchForTrouble } from '../support/app';
import { archiveClient, archiveState } from '../support/world';

test.describe('clients', () => {
  test('add, find, edit and archive a client', async ({ page }) => {
    const { problems } = watchForTrouble(page);
    acceptDialogs(page);
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
    await searchList(page, 'clientlist', lastName);
    const row = listRows(page, 'clientlist', lastName);
    await expect(row).toHaveCount(1);

    // edit
    await row.locator('a').first().click();
    await expect(page.locator('#client [name="LastName"]')).toHaveValue(lastName);
    await setField(page, '#client', 'City', 'Austin');
    expectSaved(await submitAndRead(page, () => page.locator('#client #save').click()));

    // the edit stuck
    await expect(page.locator('#clientlist')).toBeVisible();
    await searchList(page, 'clientlist', lastName);
    await row.locator('a').first().click();
    await expect(page.locator('#client [name="City"]')).toHaveValue('Austin');
    await page.locator('#client #cancel').click();

    // archive: the row stays findable by search, marked as archived
    await expect(page.locator('#clientlist')).toBeVisible();
    expect(await archiveClient(page, lastName)).toBe('archived');
    await searchList(page, 'clientlist', lastName);
    expect(await archiveState(page, lastName)).toBe('UnArchive');

    expect(problems, problems.join('\n')).toEqual([]);
  });
});

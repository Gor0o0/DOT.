let activeRequest = null;

async function loadStudents()
{
    await loadTable('/students/all', 'studentsTable', [
        'studentId', 'firstName', 'lastName', 'age'
    ]);
}

async function loadGroups()
{
    await loadTable('/groups/all', 'groupsTable', [
        'groupId', 'groupName'
    ]);
}

async function loadTable(url, tableId, fields)
{
    // Cancel previous request on repeated click or table change
    if (activeRequest) activeRequest.abort();

    const controller = new AbortController();
    activeRequest = controller;

    ClearPage();

    const table = document.getElementById(tableId);
    const tbody = table.querySelector('tbody');
    const status = document.getElementById('loadStatus');

    tbody.replaceChildren();
    status.textContent = 'Loading...';

    try
    {
        const response = await fetch(url, { signal: controller.signal });

        if (!response.ok)
        {
            throw new Error(`HTTP error: ${response.status}`);
        }

        const items = await response.json();

        if (!Array.isArray(items) || items.some(item =>
            item === null || typeof item !== 'object' || Array.isArray(item)
        ))
        {
            throw new Error('Server returned data in an invalid format');
        }

        // Old response must not change the interface after a new click
        if (activeRequest !== controller) return;

        const fragment = document.createDocumentFragment();

        for (const item of items)
        {
            const row = document.createElement('tr');

            for (const field of fields)
            {
                const cell = document.createElement('td');
                cell.textContent = item[field] ?? '';
                row.appendChild(cell);
            }

            fragment.appendChild(row);
        }

        tbody.replaceChildren(fragment);
        table.style.display = 'table';
        status.textContent = items.length === 0 ? 'No records yet' : '';
    }
    catch (error)
    {
        if (controller.signal.aborted || activeRequest !== controller) return;

        status.textContent = 'Failed to load data, try again';
        console.error('Loading error:', error);
    }
    finally
    {
        if (activeRequest === controller) activeRequest = null;
    }
}

function ClearPage()
{
    document.getElementById('studentsTable').style.display = 'none';
    document.getElementById('groupsTable').style.display = 'none';
}
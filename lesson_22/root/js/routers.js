document.addEventListener('DOMContentLoaded', () =>
{
    const forms = document.querySelectorAll('main form');
    const actions = [addGroup, addStudent, updateStudent, deleteStudent];

    forms.forEach((form, index) =>
    {
        form.addEventListener('submit', event =>
        {
            event.preventDefault();
            actions[index]();
        });
    });

    loadStudents();
});

function showMessage(text, error = false)
{
    const message = document.getElementById('gr');
    message.textContent = text;
    message.className = error ? 'error' : 'success';
}

async function request(url, method = 'GET', data = null)
{
    const options = { method: method };

    if (data !== null)
    {
        options.headers = { 'Content-Type': 'application/json' };
        options.body = JSON.stringify(data);
    }

    const response = await fetch(url, options);
    const text = await response.text();
    let result = null;

    try
    {
        result = text ? JSON.parse(text) : null;
    }
    catch
    {
        if (response.ok) throw new Error('The server returned non-JSON data. Check the request URL');
    }

    if (!response.ok)
    {
        const description = result && (result.message || result.detail || result.title);
        throw new Error(description || `Request error: ${response.status}`);
    }

    return result;
}

function addCell(row, value)
{
    const cell = row.insertCell();

    cell.textContent = value;
}

function ClearPage()
{
    document.getElementById('studentsTable').style.display = 'none';
    document.getElementById('groupsTable').style.display = 'none';
}

async function loadStudents()
{
    ClearPage();

    try
    {
        const students = await request('/students/all');

        if (!Array.isArray(students)) throw new Error('The server did not return a list of students');

        const body = document.getElementById('studentBody');
        body.replaceChildren();

        for (const student of students)
        {
            const row = body.insertRow();

            addCell(row, student.studentId);
            addCell(row, student.firstName);
            addCell(row, student.lastName);
            addCell(row, student.age);
        }

        document.getElementById('studentsTable').style.display = 'table';
        setActiveTab(true);

        return students;
    }
    catch (error)
    {
        showMessage(error.message, true);
        return null;
    }
}

async function loadGroups()
{
    ClearPage();

    try
    {
        const groups = await request('/groups/all');

        if (!Array.isArray(groups)) throw new Error('The server did not return a list of groups');

        const body = document.getElementById('groupBody');
        body.replaceChildren();

        for (const group of groups)
        {
            const row = body.insertRow();

            addCell(row, group.groupId);
            addCell(row, group.groupName);
        }

        document.getElementById('groupsTable').style.display = 'table';
        setActiveTab(false);

        return groups;
    }
    catch (error)
    {
        showMessage(error.message, true);
        return null;
    }
}

function setActiveTab(students)
{
    const buttons = document.querySelectorAll('header button');

    buttons[0].classList.toggle('active', students);
    buttons[1].classList.toggle('active', !students);

    buttons[0].setAttribute('aria-pressed', students);
    buttons[1].setAttribute('aria-pressed', !students);
}

function readStudent(editing = false)
{
    const student = {
        firstName: document.getElementById(editing ? 'firstNameUpdate' : 'firstNameStudent').value.trim(),
        lastName: document.getElementById(editing ? 'lastNameUpdate' : 'lastNameStudent').value.trim(),
        age: Number(document.getElementById(editing ? 'ageUpdate' : 'ageStudent').value),
        groupId: Number(document.getElementById(editing ? 'groupIdUpdate' : 'groupId').value)
    };

    if (!student.firstName || student.firstName.length > 50 ||
        !student.lastName || student.lastName.length > 50)
    {
        throw new Error('First name and last name must contain 1 to 50 characters');
    }

    if (!Number.isInteger(student.age) || student.age < 1 || student.age > 120)
    {
        throw new Error('Enter an integer age between 1 and 120');
    }

    if (!Number.isInteger(student.groupId) || student.groupId < 1)
    {
        throw new Error('Enter a positive integer ID of an existing group');
    }

    return student;
}

function readId(elementId)
{
    const id = Number(document.getElementById(elementId).value);

    if (!Number.isInteger(id) || id < 1) throw new Error('Enter a positive integer student ID');

    return id;
}

function setBusy(sectionIndex, busy)
{
    const section = document.querySelectorAll('main section')[sectionIndex];

    section.querySelectorAll('button').forEach(button => button.disabled = busy);
}

async function addGroup()
{
    setBusy(0, true);

    try
    {
        const name = document.getElementById('nameGroup').value.trim();

        if (!name || name.length > 20) throw new Error('Group name must contain 1 to 20 characters');

        await request('/groups/add', 'POST', { groupName: name });

        document.getElementById('nameGroup').value = '';
        showMessage('Group added');

        await loadGroups();
    }
    catch (error)
    {
        showMessage(error.message, true);
    }
    finally
    {
        setBusy(0, false);
    }
}

async function addStudent()
{
    setBusy(1, true);

    try
    {
        const student = readStudent();
        const result = await request('/students/add', 'POST', student);

        document.getElementById('firstNameStudent').closest('form').reset();
        showMessage(`Student added. ID: ${result.studentId}`);

        await loadStudents();
    }
    catch (error)
    {
        showMessage(error.message, true);
    }
    finally
    {
        setBusy(1, false);
    }
}

async function updateStudent()
{
    setBusy(2, true);

    try
    {
        const id = readId('idStudentUpdate');
        const student = readStudent(true);

        await request(`/students/${id}`, 'PUT', student);

        const students = await loadStudents();

        if (students === null) return;

        const saved = students.find(s => s.studentId === id);

        if (!saved || saved.firstName !== student.firstName || saved.lastName !== student.lastName ||
            saved.age !== student.age || saved.groupId !== student.groupId)
        {
            throw new Error('The server accepted the request, but the data did not change. Check PUT in Program.cs');
        }

        showMessage('Student data saved');
    }
    catch (error)
    {
        showMessage(error.message, true);
    }
    finally
    {
        setBusy(2, false);
    }
}

async function deleteStudent()
{
    setBusy(3, true);

    try
    {
        const id = readId('idStudentDelete');

        if (!confirm(`Delete student with ID ${id}?`)) return;

        await request(`/students/${id}`, 'DELETE');

        const students = await loadStudents();

        if (students === null) return;

        if (students.some(s => s.studentId === id))
        {
            throw new Error('The server accepted the request, but the student was not deleted. Check DELETE in Program.cs');
        }

        document.getElementById('idStudentDelete').value = '';
        showMessage('Student deleted');
    }
    catch (error)
    {
        showMessage(error.message, true);
    }
    finally
    {
        setBusy(3, false);
    }
}
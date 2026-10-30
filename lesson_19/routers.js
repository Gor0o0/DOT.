let editingId = null;
let currentPage = "students";
const $ = id => document.getElementById(id);

document.addEventListener("DOMContentLoaded", () =>
{
    $("studentsButton").addEventListener("click", () => showPage("students"));
    $("groupsButton").addEventListener("click", () => showPage("groups"));
    $("studentForm").addEventListener("submit", saveStudent);
    $("groupForm").addEventListener("submit", addGroup);
    $("cancelEditButton").addEventListener("click", resetStudentForm);

    loadGroups();
    loadStudents();
});

async function api(url, options = {})
{
    const response = await fetch(url, {
        ...options,
        headers: { ...(options.body ? { "Content-Type": "application/json" } : {}), ...options.headers }
    });

    if (!response.ok)
    {
        let message = `Error: ${response.status}`;

        try
        {
            const data = await response.json();
            message = data.message || data.detail || (typeof data === "string" ? data : message);
        }
        catch
        {
            try
            {
                message = await response.text() || message;
            }
            catch {}
        }

        throw new Error(message);
    }

    if (response.status === 204) return null;

    const type = response.headers.get("content-type") || "";
    return type.includes("application/json") ? response.json() : null;
}

function message(text, error = false)
{
    $("message").textContent = text;
    $("message").className = error ? "error" : "success";
}

function showPage(page)
{
    currentPage = page;

    $("studentsTable").hidden = page !== "students";
    $("groupsTable").hidden = page !== "groups";
    $("studentSection").hidden = page !== "students";
    $("groupSection").hidden = page !== "groups";

    $("studentsButton").classList.toggle("active", page === "students");
    $("groupsButton").classList.toggle("active", page === "groups");

    message("");

    if (page === "students") loadStudents();
    else loadGroups();
}

async function loadStudents()
{
    try
    {
        const students = await api("/students/all");
        const body = $("studentBody");

        body.replaceChildren();

        students.forEach(s =>
        {
            const row = document.createElement("tr");

            [s.studentId, s.firstName, s.lastName, s.age, s.groupId].forEach(value =>
            {
                const td = document.createElement("td");
                td.textContent = value ?? "";
                row.append(td);
            });

            const actions = document.createElement("td");

            const edit = document.createElement("button");
            edit.textContent = "Edit";
            edit.type = "button";
            edit.addEventListener("click", () => editStudent(s));

            const del = document.createElement("button");
            del.textContent = "Delete";
            del.type = "button";
            del.className = "delete";
            del.addEventListener("click", () => deleteStudent(s));

            actions.append(edit, del);
            row.append(actions);
            body.append(row);
        });
    }
    catch (e)
    {
        message(e.message, true);
    }
}

async function loadGroups()
{
    try
    {
        const groups = await api("/groups/all");
        const body = $("groupBody");

        body.replaceChildren();

        const select = $("groupId");
        const selected = select.value;

        select.replaceChildren(new Option("Select group", ""));

        groups.forEach(g =>
        {
            const row = document.createElement("tr");

            [g.groupId, g.groupName].forEach(value =>
            {
                const td = document.createElement("td");
                td.textContent = value ?? "";
                row.append(td);
            });

            body.append(row);
            select.add(new Option(`${g.groupName} (ID: ${g.groupId})`, g.groupId));
        });

        if (selected) select.value = selected;
    }
    catch (e)
    {
        message(e.message, true);
    }
}

async function saveStudent(event)
{
    event.preventDefault();

    const student = {
        firstName: $("firstNameStudent").value.trim(),
        lastName: $("lastNameStudent").value.trim(),
        age: Number($("ageStudent").value),
        groupId: Number($("groupId").value)
    };

    if (!student.firstName || !student.lastName || !student.groupId ||
        !Number.isInteger(student.age) || student.age < 1 || student.age > 120)
    {
        message("Check first name, last name, age and group", true);
        return;
    }

    try
    {
        if (editingId === null)
        {
            const id = await api("/students/add", { method: "POST", body: JSON.stringify(student) });
            message(`Student added. ID: ${id}`);
        }
        else
        {
            await api(`/students/${editingId}`, { method: "PUT", body: JSON.stringify(student) });
            message("Student data updated");
        }

        resetStudentForm();
        await loadStudents();
    }
    catch (e)
    {
        message(e.message, true);
    }
}

function editStudent(s)
{
    editingId = s.studentId;

    $("firstNameStudent").value = s.firstName;
    $("lastNameStudent").value = s.lastName;
    $("ageStudent").value = s.age;
    $("groupId").value = s.groupId;

    $("studentFormTitle").textContent = "Edit student";
    $("saveStudentButton").textContent = "Save";
    $("cancelEditButton").hidden = false;

    $("studentSection").scrollIntoView({ behavior: "smooth" });
}

async function deleteStudent(s)
{
    if (!confirm(`Delete student ${s.firstName} ${s.lastName}?`)) return;

    try
    {
        await api(`/students/${s.studentId}`, { method: "DELETE" });
        message("Student deleted");
        await loadStudents();
    }
    catch (e)
    {
        message(e.message, true);
    }
}

function resetStudentForm()
{
    editingId = null;

    $("studentForm").reset();
    $("studentFormTitle").textContent = "Add student";
    $("saveStudentButton").textContent = "Add";
    $("cancelEditButton").hidden = true;
}

async function addGroup(event)
{
    event.preventDefault();

    const groupName = $("groupName").value.trim();

    if (!groupName)
    {
        message("Enter group name", true);
        return;
    }

    try
    {
        const id = await api("/groups/add", {
            method: "POST",
            body: JSON.stringify({ groupName })
        });

        $("groupForm").reset();

        await loadGroups();
        message(`Group added. ID: ${id}`);
    }
    catch (e)
    {
        message(e.message, true);
    }
}
var id = null;
var Ids = [];
var fillSelectsForAdd = function (mainSelector, selector, url) {
    $(mainSelector).change(function () {
        var id = $(this).val();
        $(selector + " option").remove();
        $.ajax({
            url: "/Payroll/Work/" + url + "/" + id,
            type: "post",
            dataType: "json",
            success: function (response) {
                var option = `<option></option>`
                $(selector).append(option);
                if (response.status == 200) {
                    for (var i = 0; i < response.data.length; i++) {
                        option = `<option value="${response.data[i].id}">${response.data[i].name}</option>`
                        $(selector).append(option);
                    }
                }
                else {
                    alert(response.message);
                }
            }
        })
    })
}
var fillSelectsForEdit = function (mainSelector, selector, url, branchId) {
    $(mainSelector).change(function () {
        var selectorid = $(this).val();
        var positionId = $(".edit-position").val();
        if (branchId == 0) {
            branchId == $(".edit-branch").val();
        }
        $(selector + " option").remove();
        $.ajax({
            url: "/Payroll/Work/" + url,
            data: { id: selectorid, positionId: positionId, branchId: branchId },
            cache: false,
            processData: false,
            contentType: false,
            type: "post",
            dataType: "json",
            success: function (response) {
                var option = `<option></option>`
                $(selector).append(option);
                if (response.status == 200) {
                    for (var i = 0; i < response.data.length; i++) {
                        if (response.data[i].id == response.id) {
                            option = `<option selected value="${response.data[i].id}">${response.data[i].name}</option>`
                            $(selector).append(option);
                        }
                        else {
                            option = `<option value="${response.data[i].id}">${response.data[i].name}</option>`
                            $(selector).append(option);
                        }
                    }
                }
                else {
                    alert(response.message);
                }
            }
        })
    })
}
var Change = function (selector, url) {
    $(selector).change(function () {
        var id = $(this).attr("id");
        var Name = $(this).val();
        $.ajax({
            url: `/Payroll/Admin/${url}?Name=${Name}&id=${id}`,
            type: "post",
            dataType: "json",
            success: function (response) {
                if (response.status == "200") {
                    $(this).val(`${Name}`);
                }
                else {
                    alert(response.message);
                }
            }
        })
    })
}
var ChangeClassId = function (Mainselector, url) {
    $(Mainselector).change(function () {
        var id = $(this).val();
        var classId = $(this).attr("id");
        $.ajax({
            url: `/Payroll/Admin/${url}/`,
            data: { id: id, classId: classId },
            type: "post",
            dataType: "json",
            success: function (response) {
                if (response.status == "400") {
                    alert(response.message);
                }
            }
        })
    })
}
var FillEmployee = function (selector, url) {
    $(selector).click(function (e) {
        e.preventDefault();
        var id = $(this).attr("id");
        $.ajax({
            url: `/Payroll/Admin/${url}/${id}`,
            type: "post",
            dataType: "json",
            success: function (response) {
                if (response.status == "200") {
                    $(".employee-details").text(`${response.data.name} ${response.data.surname}`);
                    $(".employee-id").attr("value", `${response.data.id}`);
                }
                else {
                    alert(response.message);
                }
            }
        })
    })
}
var Click = function (firstSelector, secondSelector) {
    $(secondSelector).click(function () {
        $(firstSelector).removeClass("d-none");
        $(secondSelector).addClass("d-none");
    })
}
$(".employees-list tbody td .select").click(function () {
    id = $(this).val();
    $.ajax({
        url: `/Payroll/Employee/ChangeViewBag/${id}`,
        type: "post",
        dataType: "json",
        success: function (response) {
            if (response.status == "400") {
                alert(response.message);
            }
        }
    })
})
$(".sub_page").click(function (e) {
    e.preventDefault();
    var href = $(this).attr("href");
    if (href == "/Payroll") {
        window.location.href = "/payroll/Hr/Index"
    }
    else if (href == "/Payroll/Hr/Add" || href.includes("Calculate") || (href.includes("Admin") && (!(href.includes("Vacation"))))) {
        window.location.href = href;
    }
    else {
        if (id == null) {
            id = prompt("Please insert employee id");
        }
        window.location.href = href + `/${id}`;
    }
})
$(".edit-position").change(function () {
    if ($(".asp-position").val() != $(this).val()) {
        $(".asp-position").val($(this).val());
        $(".hidden").removeClass("d-none");
    }
})
$(".photo").change(function () {
    var files = $("input[name='Photo'].photo").get(0).files;
    var file = files[0];
    var formData = new FormData();
    formData.append("Photo[]", file);
    formData.append("Photo[]", $(".photo_link").val())
    $.ajax({
        url: "/Payroll/Hr/GetPhotoLink",
        data: formData,
        cache: false,
        processData: false,
        contentType: false,
        type: "post",
        dataType: "json",
        success: function (response) {
            if (response.status == 200) {
                $(".photo_link").val(response.data);
            }
            else {
                alert("We have problem with photo");
            }
        }
    })
})
$(".worker-id").click(function () {
    Ids.push($(this).val());
})
$(".calculate").click(function (e) {
    e.preventDefault();
    var ids = Ids.join(",");
    var url = `/Payroll/Calculate/Calculate?Ids=${ids}`;
    window.location.href = url;
})
$(".months").change(function () {
    var Month = $(this).val();
    window.location.href = `/Payroll/Hr/Absent?Month=${Month}`;
})
Change(".holding-name", "ChangeHolding");
Change(".company-name", "ChangeCompany");
Change(".branch-name", "ChangeBranchName");
Change(".branch-address", "ChangeBranchAddress");
Change(".department-name", "ChangeDepartment");
Change(".position-name", "ChangePositionName");
Change(".salary-price", "ChangeSalaryPrice");
Click(".holding-name", ".click-holding-name");
Click(".company-name", ".click-company-name");
Click(".branch-name", ".click-branch-name");
Click(".branch-address", ".click-branch-address");
Click(".department-name", ".click-department-name");
Click(".position-name", ".click-position-name");
Click(".salary-price", ".click-salary-price");
Click(".company-holding", ".click-company-holding");
Click(".branch-company", ".click-branch-company");
Click(".position-department", ".click-position-department");
Click(".position-salary", ".click-position-salary");
FillEmployee(".role-modal", "AddRole");
FillEmployee(".vacation", "GetEmployee");
ChangeClassId(".company-holding", "ChangeCompanyHolding");
ChangeClassId(".branch-company", "ChangeBranchCompany");
ChangeClassId(".position-department", "ChangePositionDepartment");
ChangeClassId(".position-salary", "ChangeSalary");
fillSelectsForAdd(".holding", ".company", "GetCompanies");
fillSelectsForAdd(".company", ".branch", "GetBranches");
fillSelectsForAdd(".department", ".position", "GetPositions");
fillSelectsForEdit(".edit-holding", ".edit-company", "GetCompaniesAndSelected");
fillSelectsForEdit(".edit-company", ".edit-branch", "GetGetBranchesAndSelected");
fillSelectsForEdit(".edit-department", ".edit-position", "GetPositionsAndSelected", 0);
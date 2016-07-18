require(["el", "coco", "text!apps/person/main/detail.html"], function (el, coco, template) {
    var detail = {
        node: template,
        ready: function () {
            var context = this.$context;
            var params = this.$params;

            var isNew = params.isNew;
            var id = 0;

            if (isNew) {
                context(".id").hide();
                context("[name=remove]").hide();
            } else {
                var sections = location.href.split("/");
                id = sections[sections.length - 1];

                $.get("/api/person/get", { id: id }).success(function (person) {
                    context(".id").text(person.Id)
                    context("[name=name]").val(person.Name);
                }).fail(function (result) {
                    context(".message").html(result.responseText);
                });
            }

            context("[name=save]").on("click", function () {
                var name = context("[name=name]").val();

                $.post(isNew ? "/api/person/add" : "/api/person/update", { id: id, name: name }).success(function (result) {
                    if (result) {
                        if (isNew) {
                            location.href = "/person";
                        } else {
                            context(".message").text("success");
                        }
                    } else {
                        context(".message").text("fail");
                    }
                }).fail(function (result) {
                    context(".message").html(result.responseText);
                });
            });

            context("[name=remove]").on("click", function () {
                var name = context("[name=name]").val();

                $.post("/api/person/remove", { id: id, name: name }).success(function (result) {
                    if (result) {
                        location.href = "/person";
                    } else {
                        context(".message").text("fail");
                    }
                }).fail(function (result) {
                    context(".message").html(result.responseText);
                });
            });
        }
    }

    var view = coco({
        model: detail,
        params: {
            isNew: location.href.toLocaleString().indexOf("/person/new") != -1
        }
    });

    $(el).replaceWith(view.el);
});
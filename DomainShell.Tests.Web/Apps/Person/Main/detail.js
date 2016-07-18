require(
    ["el", "coco", "text!apps/person/main/detail.html", "apps/person/parts/error"],
    function (el, coco, template, error) {
        var detail = {
            node: template,
            components: {
                errorName: {
                    model: error
                }
            },
            ready: function () {
                var self = this;               

                var isNew = self.$params.isNew;
                var id = 0;

                if (isNew) {
                    self.$context(".id").hide();
                    self.$context("[name=remove]").hide();
                } else {
                    var sections = location.href.split("/");
                    id = sections[sections.length - 1];

                    $.get("/api/person/get", { id: id }).success(function (person) {
                        self.$context(".id").text(person.Id)
                        self.$context("[name=name]").val(person.Name);
                    }).fail(function (result) {
                        $("body").html(result.responseText);
                    });
                }

                self.$context("[name=save]").on("click", function () {
                    self.components.errorName.view.clear();

                    var name = self.$context("[name=name]").val();

                    if (name == "") {
                        self.components.errorName.view.message("not set name.");
                        alert("exist error.");
                        return;
                    }

                    $.post(isNew ? "/api/person/add" : "/api/person/update", { id: id, name: name }).success(function (result) {
                        if (result) {
                            if (isNew) {
                                location.href = "/person";
                            } else {
                                alert("success.");                            
                            }
                        } else {
                            alert("failed.");
                        }
                    }).fail(function (result) {
                        $("body").html(result.responseText);
                    });
                });

                self.$context("[name=remove]").on("click", function () {
                    self.components.errorName.view.clear();

                    var name = self.$context("[name=name]").val();

                    $.post("/api/person/remove", { id: id, name: name }).success(function (result) {
                        if (result) {
                            location.href = "/person";
                        } else {
                            alert("failed.");
                        }
                    }).fail(function (result) {
                        $("body").html(result.responseText);
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
    }
);
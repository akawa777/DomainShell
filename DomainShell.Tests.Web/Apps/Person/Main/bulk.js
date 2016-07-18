require(
    ["el", "coco", "text!apps/person/main/bulk.html", "apps/person/parts/tr", "text!apps/person/parts/tr.check.html"],
    function (el, coco, template, tr, checkTemplate) {
        var checkTr = coco.extend(tr, {
            node: checkTemplate,
            ready: function () {
                var self = this;

                self.base().ready();

                self.Id = self.$params.Id;
                self.Name = self.$params.Name;
            },
            checked: function () {
                var self = this;
                return self.$context('.target:checked').length > 0
            },
            check: function (checked) {
                var self = this;
                self.$context('.target').prop("checked", checked);
            },
            Id: "",
            Name: ""
        });

        coco.container.set("checkTr", checkTr);

        var bulk = {
            node: template,
            ready: function () {
                var self = this;

                var trViews = [];

                $.get("/api/person/getall").success(function (persons) {
                    for (var id in persons) {
                        view = self.$coco({
                            model: "checkTr",
                            params: persons[id]
                        });

                        self.$context("table tbody").append(view.el);

                        trViews.push(view);
                    }
                }).fail(function (result) {
                    self.$context(".message").html(result.responseText);
                });

                self.$context("[name=save]").on("click", function () {
                    var ids = [];

                    trViews.forEach(function (trView) {
                        if (trView.checked()) {
                            ids.push(trView.Id);
                        }
                    });

                    if (ids.length == 0) {
                        self.$context(".message").text("no select target.");
                        return;
                    }

                    var name = self.$context("input[name=name]").val();

                    if (name == "") {
                        self.$context(".message").text("no set name.");
                        return;
                    }

                    $.post("/api/person/bulkupdate", { ids: ids, name: name }).success(function (result) {
                        if (result.Success) {
                            location.href = "/person";
                        } else {
                            self.$context(".message").text("fail");
                        }
                    }).fail(function (result) {
                        self.$context(".message").html(result.responseText);
                    });
                });

                self.$context(".allTarget").on("click", function () {
                    var checked = $(this).is(':checked');

                    trViews.forEach(function (trView) {
                        trView.check(checked);
                    });
                });
            }
        }

        var view = coco({
            model: bulk
        });

        $(el).replaceWith(view.el);
    }
);
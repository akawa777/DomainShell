require(
    ["el", "coco", "text!apps/person/main/bulk.html", "apps/person/parts/tr.check"],
    function (el, coco, template, trCheck) {
        var bulk = {
            node: template,
            ready: function () {
                var self = this;

                var trViews = [];

                $.get("/api/person/getall").success(function (persons) {
                    for (var id in persons) {
                        view = self.$coco({
                            model: trCheck,
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
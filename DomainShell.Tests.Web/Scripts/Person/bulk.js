require(
    ["el", "coco", "text!/views/person/bulk", "person/tr.check", "shared/error", "person/modal"],
    function (el, coco, template, trCheck, error, modal) {
        var bulk = {
            node: template,            
            init: function() {
                var self = this;

                self.$components = {
                    errorTable: {
                        model: error
                    },
                    errorName: {
                        model: error
                    },
                    modal: {
                        model: modal,
                        params: {
                            nameTextEl: self.$context("[name=name]")
                        }
                    }
                };
            },
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
                    $("body").html(result.responseText);
                });

                self.$context("[name=save]").on("click", function () {
                    self.$components.errorTable.view.clear();
                    self.$components.errorName.view.clear();

                    var ids = [];

                    trViews.forEach(function (trView) {
                        if (trView.checked()) {
                            ids.push(trView.Id);
                        }
                    });

                    var valid = true;

                    if (ids.length == 0) {
                        self.$components.errorTable.view.message("no select target.");
                        valid = false;
                    }

                    var name = self.$context("input[name=name]").val();

                    if (name == "") {
                        self.$components.errorName.view.message("no set name.");
                        valid = false;
                    }

                    if (!valid) {
                        alert("exist error.");
                        return;
                    }

                    $.post("/api/person/bulkupdate", { ids: ids, name: name }).success(function (result) {
                        if (result.Success) {
                            location.href = "/person";
                        } else {
                            alert("failed.");
                        }
                    }).fail(function (result) {
                        $("body").html(result.responseText);
                    });
                });

                self.$context(".allTarget").on("click", function () {
                    var checked = $(this).is(':checked');

                    trViews.forEach(function (trView) {
                        trView.check(checked);
                    });
                });

                self.$context("[name=modal]").on("click", function () {
                    self.$components.modal.view.show();
                });
            }
        }

        var view = coco({
            model: bulk
        });

        $(el).replaceWith(view.el);
    }
);
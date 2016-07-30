require(
    ["el", "coco", "text!/views/person/bulk", "person/tr.check", "shared/error", "person/modal"],
    function (el, coco, template, trCheck, error, modal) {        
        var bulk = {
            node: template,                        
            ready: function () {
                var self = this;

                var trViews = [];

                $.get("/api/person/getall").success(function (persons) {
                    self.$context("table tbody").empty();
                    persons.forEach(function (person) {
                        var view = self.$coco({
                            model: trCheck,
                            params: person
                        });

                        self.$context("table tbody").append(view.el);
                        trViews.push(view);
                    });
                }).fail(function (result) {
                    $("body").html(result.responseText);
                });

                var modalView = self.$coco({
                    model: modal,
                    params: {
                        nameTextEl: self.$context("[name=name]")
                    }
                });

                self.$context().append(modalView.el);

                self.$context("[name=modal]").on("click", function () {
                    modalView.show();
                });

                var errorTableView = self.$coco({
                    model: error,
                    params: {
                        verify: function (message) {
                            var ids = [];

                            trViews.forEach(function (trView) {
                                if (trView.checked()) {
                                    ids.push(trView.Id);
                                }
                            });

                            if (ids.length == 0) {
                                message("no select target.");
                                return false;
                            }

                            return true;
                        }
                    }
                });

                self.$context("table").before(errorTableView.el);

                var errorNameView = self.$coco({
                    model: error,
                    params: {
                        verify: function (message) {
                            var name = self.$context("input[name=name]").val();

                            if (name == "") {
                                message("no set name.");
                                return false;
                            }

                            return true;
                        }
                    }
                });

                self.$context("input").after(errorNameView.el);

                self.$context("[name=save]").on("click", function () {
                    errorTableView.clear();
                    errorNameView.clear();

                    var vald = true;

                    if (!errorTableView.verify()) {
                        vald = false;
                    }

                    if (!errorNameView.verify()) {
                        vald = false;
                    }

                    if (!vald) {
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
            }
        }

        var view = coco({
            model: bulk
        });

        $(el).replaceWith(view.el);
    }
);
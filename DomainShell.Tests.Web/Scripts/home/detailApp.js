define([], function (vmustache) {
    var detailApp = {
        template: "",
        data: {
            id: "",
            name: "",            
            visible: false
        },
        detailHook: function (el) {
            this.modal = function (args) {
                if (args) {
                    $(el).modal(args);
                } else {
                    $(el).modal(args);
                }
            }
        },        
        change: function (render, event, item) {
            item.name = event.target.value;
        },
        add: function (render, event, item) {
            var self = this;
            var createCommand = item;

            $.ajax({
                url: "/home/create",
                method: "post",
                data: createCommand,
            }).done(function (data) {
                self.modal('hide');
                self.list(render);
            }).fail(function (data) {
                alert("error!");
            });
        },
        update: function (render, event, item) {
            var self = this;
            var personQuery = { id: item.id };

            var updateCommand = item;

            $.ajax({
                url: "/home/update",
                method: "post",
                data: updateCommand,
            }).done(function (data) {
                self.modal('hide');
                self.list(render);
            }).fail(function (data) {
                alert("error!");
            });
        },
        remove: function (render, event, item) {
            var self = this;
            var removeCommand = item;

            $.ajax({
                url: "/home/remove",
                method: "post",
                data: removeCommand,
            }).done(function (data) {
                self.modal('hide');
                self.list(render);
            }).fail(function (data) {
                alert("error!");
            });
        },
        init: function (render, options) {
            var self = this;
            self.data.id = options.data.id;
            self.data.visible = options.data.visible;
            self.list = options.list;

            if (self.data.visible) {
                $.get("/scripts/home/detail.html").done(function (template) {
                    self.template = template;

                    if (self.data.id == "") {
                        self.data.id = "";
                        self.data.name = "";

                        self.data.displayForAdd = "inline";
                        self.data.displayForUpdate = "none";

                        render(function () {
                            self.modal();
                        });
                    } else {
                        var personQuery = { id: self.data.id };

                        $.ajax({
                            url: "/home/load",
                            method: "post",
                            data: personQuery,
                        }).done(function (data) {
                            self.data.id = data.Id;
                            self.data.name = data.Name;

                            self.data.displayForAdd = "none";
                            self.data.displayForUpdate = "inline";

                            render(function () {
                                self.modal();
                            });

                        }).fail(function (data) {
                            alert("error!");
                        });
                    }
                });
            } else {
                render();
            }
        }
    }

    return detailApp;
});
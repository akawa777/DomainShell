define([], function () {
    var detailApp = {
        template: "",
        data: {
            id: "",
            name: "",            
            visible: false,
            displayForAdd: "",
            displayForUpdate: ""
        },
        detailHook: function (el, adapt) {
            var self = this;

            self.modal = function (args) {                
                if (args) {
                    $(el).modal(args);
                } else {
                    $(el).modal(args);
                }
            }
        },        
        keyup: function (adapt, event, item) {           
            item.name = event.target.value;
            item.isChange = false;
            adapt();
        },
        add: function (adapt, event, item) {
            var self = this;
            var createCommand = item;

            $.ajax({
                url: "/home/create",
                method: "post",
                data: createCommand,
            }).done(function (data) {
                if (data) {
                    self.modal('hide');
                    self.list(adapt);
                } else {
                    alert("error!");
                }
            }).fail(function (data) {
                alert("error!");
            });
        },
        update: function (adapt, event, item) {
            var self = this;
            var personQuery = { id: item.id };

            var updateCommand = item;

            $.ajax({
                url: "/home/update",
                method: "post",
                data: updateCommand,
            }).done(function (data) {
                if (data) {
                    self.modal('hide');
                    self.list(adapt);
                } else {
                    alert("error!");
                }
            }).fail(function (data) {
                alert("error!");
            });
        },
        remove: function (adapt, event, item) {
            var self = this;
            var removeCommand = item;

            $.ajax({
                url: "/home/remove",
                method: "post",
                data: removeCommand,
            }).done(function (data) {
                if (data) {
                    self.modal('hide');
                    self.list(adapt);
                } else {
                    alert("error!");
                }
            }).fail(function (data) {
                alert("error!");
            });
        },
        show: function (adapt) {
            var self = this;

            if (self.data.isChange) {
                if (self.data.id == "") {
                    self.data.id = "";
                    self.data.name = "";

                    self.data.displayForAdd = "inline";
                    self.data.displayForUpdate = "none";

                    adapt(function (el) {
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

                        adapt(function () {
                            self.modal();
                        });

                    }).fail(function (data) {
                        alert("error!");
                    });
                }
            } else {
                adapt();

            }
        },
        init: function (adapt, options) {
            var self = this;
            self.data = options.data;
            self.list = options.list;            

            if (self.data.visible) {
                if (self.template == "") {
                    $.get("/scripts/home/detail.html?bust=v3").done(function (template) {
                        self.template = template;
                        self.show(adapt);
                    });
                } else {
                    self.show(adapt);
                }
                
            } else {
                adapt();
            }
        }
    }

    return detailApp;
});
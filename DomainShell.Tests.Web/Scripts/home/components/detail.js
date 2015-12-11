define([], function () {
    var detailApp = {
        template: "",
        data: {
            id: "",
            name: "",
            displayForAdd: "",
            displayForUpdate: ""
        },
        change: function (adapt, event, item) {
            item.name = event.target.value;                        
        },
        add: function (adapt, event, item, options) {
            this.save(adapt, item, "create", options);
        },
        update: function (adapt, event, item, options) {
            this.save(adapt, item, "update", options);
        },
        remove: function (adapt, event, item, options) {
            this.save(adapt, item, "remove", options);
        },
        save: function (adapt, item, action, options) {
            var self = this;

            var command = item;

            $.ajax({
                url: "/home/" + action,
                method: "post",
                data: command,
            }).done(function (data) {
                if (data) {                    
                    options.loadList(adapt);
                } else {
                    alert("error!");
                }
            }).fail(function (data) {
                alert("error!");
            });
        },
        show: function (adapt) {
            var self = this;

            if (self.data.id == "") {
                self.data.id = "";
                self.data.name = "";

                self.data.displayForAdd = "inline";
                self.data.displayForUpdate = "none";

                adapt();
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

                    adapt();

                }).fail(function (data) {
                    alert("error!");
                });
            }
        },
        init: function (adapt, options) {
            var self = this;
            self.data = options.data;            

            if (self.template == "") {
                $.get("/scripts/home/templates/detail.html?bust=v2").done(function (template) {
                    self.template = template;
                    self.show(adapt);
                });
            } else {
                self.show(adapt);
            }
        }
    }

    return detailApp;
});
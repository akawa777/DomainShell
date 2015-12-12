define(["text!home/templates/detail.html"], function (template) {
    var detailApp = {
        template: template,
        data: {
            id: "",
            name: "",
            displayForAdd: "",
            displayForUpdate: ""
        },
        change: function (adapt, event, item) {
            item.name = event.target.value;            
            adapt();
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
            }).fail(function (err) {
                alert(err.responseText);
            });
        },
        show: function (adapt, doRerender) {
            var self = this;

            if (doRerender) {
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
                        alert(err.responseText);
                    });
                }
            } else {
                adapt();
            }
        },
        init: function (adapt, options) {
            var self = this;
            self.data.id = options.data.id;
            
            self.show(adapt, options.doRerender);
        }
    }

    return detailApp;
});
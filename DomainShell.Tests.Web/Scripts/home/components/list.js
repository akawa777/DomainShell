define(["home/components/detail", "text!home/templates/list.html"], function (detail, template) {
    var listApp = {        
        template: template,
        data: {
            list: [],
            detail: {
                id: ""
            }
        },
        componetns: {
            detail: detail
        },
        modal: function(args) {

        },
        detailHook: function (el) {
            var self = this;

            self.modal = function (args) {
                if (args) {
                    $(el).modal(args);
                } else {
                    $(el).modal(args);
                }
            }
        },
        clickCreate: function (adapt, event, item) {
            var self = this;
            self.data.detail.id = "";
            self.doRerenderDetail = true;

            adapt(function () {
                self.modal();
            });
        },
        clickDetail: function (adapt, event, item) {
            var self = this;
            self.data.detail.id = item.id;
            self.doRerenderDetail = true;

            var self = this;
            adapt(function () {
                self.modal();
            });
        },
        doRerenderDetail: true,
        loadList: function (adapt) {
            var self = this;

            self.modal("hide");

            $.ajax({
                url: "/home/list",
                method: "post"
            }).done(function (rtnData) {
                self.data.list = [];

                rtnData.forEach(function (item) {
                    self.data.list.push({
                        id: item.Id,
                        name: item.Name
                    });
                });

                self.data.detail.id = "";                

                adapt();
            }).fail(function (data) {
                alert(err.responseText);
            });
        },
        detailComponent: function(render, item, options) {            
            var self = this;

            var loadList = function (adapt) {
                self.loadList(adapt);
            }

            render(self.componetns.detail, { 
                data: self.data.detail, 
                loadList: loadList,
                doRerender: self.doRerenderDetail
            });

            self.doRerenderDetail = false;
        },
        init: function (adapt) {
            var self = this;
            self.loadList(adapt);
        }
    }    

    return listApp;
});
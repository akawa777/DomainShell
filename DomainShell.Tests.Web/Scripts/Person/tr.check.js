define(["coco", "person/tr", "text!/person/trcheck"], function (coco, tr, template) {
    var trCheck = coco.extend(tr, {
        node: template,
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

    return trCheck
});
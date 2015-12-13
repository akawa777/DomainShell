define(["text!dashboard/templates/user.html"], function (template) {
    var user = {
        template: template,
        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July"],
            datasets: [
                {
                    label: "My First dataset",
                    fillColor: "rgba(220,220,220,0.2)",
                    strokeColor: "rgba(220,220,220,1)",
                    pointColor: "rgba(220,220,220,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(220,220,220,1)",
                    data: [65, 59, 80, 81, 56, 55, 40]
                },
                {
                    label: "My Second dataset",
                    fillColor: "rgba(151,187,205,0.2)",
                    strokeColor: "rgba(151,187,205,1)",
                    pointColor: "rgba(151,187,205,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(151,187,205,1)",
                    data: [28, 48, 40, 19, 86, 27, 90]
                }
            ]
        },
        canvas: function (el) {
            var self = this;

            self.Line = function () {
                var ctx = el.getContext("2d");
                el.setAttribute("width", el.parentNode.clientWidth);
                el.setAttribute("height", 300);
                var myLineChart = new Chart(ctx).Line(self.data);

                var timer = false;
                $(window).resize(function () {
                    if (timer !== false) {
                        clearTimeout(timer);
                    }
                    timer = setTimeout(function () {
                        myLineChart.destroy();
                        ctx = el.getContext("2d");
                        el.setAttribute("width", el.parentNode.clientWidth);
                        el.setAttribute("height", 300);
                        myLineChart = new Chart(ctx).Line(self.data, {
                            animation: false
                        });
                    }, 200);
                });
            }
            
        },
        init: function (adapt, options) {
            var self = this;

            adapt(function () {
                self.Line();
            });
        }
    }

    return user;
});
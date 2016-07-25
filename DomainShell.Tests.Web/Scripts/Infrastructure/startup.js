require.config({
    baseUrl: "/Scripts",    
    paths: {
        text: "/Scripts/Infrastructure/text",
        coco: "/Scripts/Infrastructure/coco"
    }
});

define("el", [], function () {
    return document.getElementById($appId);
});
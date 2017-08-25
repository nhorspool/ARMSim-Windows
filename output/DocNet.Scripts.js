function ToggleSidebarNode(clicked) {
    var node = $(clicked);
    var glyph = node.find("span");
    glyph.toggleClass("glyphicon-menu-right");
    glyph.toggleClass("glyphicon-menu-down");
    var childList = node.siblings(".child-list");
    childList.collapse('toggle');
}
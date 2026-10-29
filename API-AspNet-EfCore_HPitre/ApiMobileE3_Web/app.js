const API_BASE_URL = "https://localhost:64984";

//Fct au démarrage (loading)
$(function () {
  let $startTuile;

  for (let y = 10; y < 15; y++) {
    for (let x = 10; x < 15; x++) {
      const $tuile = creerTuile(x, y);
      if (x === 10 && y === 10) {
        $startTuile = $tuile;
      }
    }
  }

  chargerTuile(10, 10, $startTuile).done(function () {
    const neighbors = getNeighboringTiles(10, 10, 50, 50);
    for (const n of neighbors) {
      if (n.x < 10 || n.x > 14 || n.y < 10 || n.y > 14) continue;

      const $neighborTuile = creerTuile(n.x, n.y);
      chargerTuile(n.x, n.y, $neighborTuile);
    }
  });
});
$("#map-grid").on("click", ".tile", function () {
  const $tuile = $(this);
  const x = $tuile.data("x");
  const y = $tuile.data("y");

  if ($tuile.hasClass("tile-loaded")) {
    afficherSelection($tuile.data("tuileInfo"), x, y);
  } else {
    chargerTuile(x, y, $tuile).done(function (data) {
      afficherSelection(data, x, y);
    });
  }
});

function afficherSelection(data, x, y) {
  $("#posTop").text(`(${x}, ${y})`);
  $("#tuile-pos-value").text(`(${x}, ${y})`);
  $("#tuile-type-value").text(data.typeTxt);
  $("#tuile-passable-value").text(data.isTraversable ? "✅" : "❌");
  $("#tuile-desc-value").text(
    data.description || `Tuile ${data.typeTxt} en position (${x},${y})`,
  );
}

function creerTuile(x, y) {
  const $existing = $(`.tile[data-x="${x}"][data-y="${y}"]`);
  if ($existing.length) return $existing;

  let $tuile = $("<button> </button>");
  $tuile.addClass("tile tile-unloaded");
  $tuile.attr("data-x", x);
  $tuile.attr("data-y", y);
  $tuile.text("?");
  $tuile.appendTo("#map-grid");
  return $tuile;
}

function chargerTuile(x, y, $tuile) {
  return $.getJSON(`${API_BASE_URL}/Tile/${x}/${y}`)
    .done(function (data) {
      $tuile.data("tuileInfo", data);
      $tuile.removeClass("tile-unloaded").addClass("tile-loaded");

      if (data.imgUrl) {
        const $img = $("<img>", {
          src: API_BASE_URL + data.imgUrl,
          alt: `${data.description} ${data.typeTxt}`,
          class: "tile-img",
        });
        $tuile.empty().append($img);
      } else {
        //fallback img not working
        $tuile.text(data.typeTxt);
      }
    })
    .fail(function (xhr, textStatus, error) {
      console.error(
        `[Grid Error] Could not load position (${x}, ${y}):`,
        error,
      );
    });
}

function getNeighboringTiles(x, y, worldWidth, worldHeight) {
  const neighbors = [];

  for (let dx = -1; dx <= 1; dx++) {
    for (let dy = -1; dy <= 1; dy++) {
      // loop over center
      if (dx === 0 && dy === 0) continue;

      const nX = x + dx;
      const nY = y + dy;

      // verify if neighbor is in boundaries
      if (nX >= 0 && nX < worldWidth && nY >= 0 && nY < worldHeight) {
        neighbors.push({ x: nX, y: nY });
      }
    }
  }

  return neighbors;
}

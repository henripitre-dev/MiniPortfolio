from django.contrib import admin
from django.urls import reverse
from django.utils.html import format_html, format_html_join
from django.utils.safestring import mark_safe
from django.utils.translation import gettext, gettext_lazy as _
from modeltranslation.admin import TranslationAdmin

from .forms import AlbumForm
from .models import Artiste, Genre, Album, AlbumUtilisateur, Song

@admin.register(Artiste)
class ArtisteAdmin(TranslationAdmin):
    list_display = ("nom", "pays", "view_albums_link")
    list_filter = ['pays',]
    ordering = ['nom',]
    search_fields = ['nom']
    readonly_fields = ("albums_list",)

    def albums_list(self, obj):
        if not obj.pk:
            return "-"

        albums = obj.albums.all()
        if not albums:
            album_links = gettext("Aucun album associé")
        else:
            album_links = format_html_join(
                mark_safe("<br>"),
                '<a href="{}">{}</a>',
                (
                    (
                        reverse("admin:vinylvault_album_change", args=[a.pk]),
                        a.titre,
                    )
                    for a in albums
                )
            )

        # Lien pour ajouter un album avec l'artiste pré-rempli
        add_url = (
            reverse("admin:vinylvault_album_add")
            + f"?artiste={obj.pk}"
        )
        add_link = format_html(
            '<br><br><a class="addlink" href="{}">{}</a>',
            add_url, gettext("Ajouter un album")
        )

        return format_html("{}{}", album_links, add_link)

    albums_list.short_description = _("Albums associés")

    def view_albums_link(self, obj):
        url = (
            reverse("admin:vinylvault_album_changelist")
            + f"?artiste__id__exact={obj.id}"
        )
        return format_html('<a href="{}">{}</a>', url, gettext("Voir les albums"))

    view_albums_link.short_description = _("Albums associés")


@admin.register(Genre)
class GenreAdmin(TranslationAdmin):
    list_display = ("nom", "view_albums_link")
    ordering = ['nom',]
    search_fields = ['nom']
    readonly_fields = ("albums_list",)

    def albums_list(self, obj):
        if not obj.pk:
            return "-"

        albums = obj.albums.all()
        if not albums:
            album_links = gettext("Aucun album associé")
        else:
            album_links = format_html_join(
                mark_safe("<br>"),
                '<a href="{}">{}</a>',
                (
                    (
                        reverse("admin:vinylvault_album_change", args=[a.pk]),
                        a.titre,
                    )
                    for a in albums
                )
            )

        # Lien pour ajouter un album avec le genre pré-rempli
        add_url = (
            reverse("admin:vinylvault_album_add")
            + f"?genre={obj.pk}"
        )
        add_link = format_html(
            '<br><br><a class="addlink" href="{}">{}</a>',
            add_url, gettext("Ajouter un album")
        )

        return format_html("{}{}", album_links, add_link)

    albums_list.short_description = _("Albums associés")

    def view_albums_link(self, obj):
        url = (
            reverse("admin:vinylvault_album_changelist")
            + f"?genre__id__exact={obj.id}"
        )
        return format_html('<a href="{}">{}</a>', url, gettext("Voir les albums"))

    view_albums_link.short_description = _("Albums associés")

class SongInline(admin.TabularInline):
    # Affichage et modification DANS l'affichage album admin
    model = Song
    #add ligne vide pour ajouter une chanson a fin
    extra = 1 

@admin.register(Album)
class AlbumAdmin(admin.ModelAdmin):
    form = AlbumForm  

    inlines = [SongInline,]
    list_editable = ['genre', 'annee_sortie',]  # modifiable direct dans la liste, sans ouvrir l'album
    list_display = ['album_titre_with_image', 'artiste', 'genre', 'annee_sortie']
    list_filter = ['genre', 'artiste',]
    ordering = ['titre',]
    readonly_fields = ['image_preview',]
    search_fields = ['titre', 'artiste__nom']

    def album_titre_with_image(self, obj):
        if obj.pochette:
            return format_html(
                '<span style="display:flex; align-items:center;">'
                '<img src="{}" style="max-width:32px; margin-right:6px;" />'
                '{}'
                '</span>',
                obj.pochette,
                obj.titre,
            )
        return obj.titre
    album_titre_with_image.short_description = _("Titre")
    album_titre_with_image.admin_order_field = 'titre'

    def image_preview(self, obj):
        if obj.pochette:
            return format_html('<img src="{}" style="max-width:200px" />', obj.pochette)
        return ""
    image_preview.short_description = _("Prévisualisation")

    def get_readonly_fields(self, request, obj=None):
        # Permet de ne pas afficher "Image Preview" lors de l'ajout
        if obj:  # si c'est pdt modif
            return self.readonly_fields
        return []


@admin.register(AlbumUtilisateur)
class AlbumUtilisateurAdmin(admin.ModelAdmin):
    #note personnelle d'un user donc l'admin ne peut pas la changer(exclude)
    exclude = ['note',]
    list_display = ['user', 'album', 'statut', 'note', 'isFavori']
    list_filter = ['statut', 'isFavori', 'user',]
    ordering = ['user', 'album',]
    search_fields = ['album__titre', 'user__username']


@admin.register(Song)
class SongAdmin(admin.ModelAdmin):
    list_display = ['titre', 'album', 'nb', 'duree']
    list_filter = ['album',]
    ordering = ['album', 'nb',]
    search_fields = ['titre']

from django.conf import settings
from django.db import models
from django.utils.translation import gettext_lazy as _

class Artiste(models.Model):
    nom = models.CharField(max_length=150, unique=True, verbose_name=_("nom"))
    pays = models.CharField(max_length=100, blank=True, verbose_name=_("pays"))

    class Meta:
        verbose_name = _("artiste")
        verbose_name_plural = _("artistes")

    def __str__(self):
        return self.nom


class Genre(models.Model):
    nom = models.CharField(max_length=100, unique=True, verbose_name=_("nom"))

    class Meta:
        verbose_name = _("genre")
        verbose_name_plural = _("genres")

    def __str__(self):
        return self.nom

# Album = shared library avec goûts uniques per user
class Album(models.Model):
    titre = models.CharField(max_length=200, verbose_name=_("titre"))
    artiste = models.ForeignKey(Artiste, on_delete=models.CASCADE, related_name='albums', verbose_name=_("artiste"))
    genre = models.ForeignKey(Genre, on_delete=models.SET_NULL, null=True, related_name='albums', verbose_name=_("genre"))
    annee_sortie = models.IntegerField(verbose_name=_("année de sortie"))

    image = models.ImageField(default="", blank=True, upload_to='albums/', verbose_name=_("image"))
    # pochette en ligne (ex: Cover Art Archive), utilisée si aucune image n'est uploadée
    image_url = models.URLField(blank=True, verbose_name=_("URL de la pochette"))

    class Meta:
        verbose_name = _("album")
        verbose_name_plural = _("albums")

    def __str__(self):
        return f"{self.titre} — {self.artiste}"

    @property
    def pochette(self):
        # image uploadée en priorité, sinon l'URL en ligne, sinon "" (le template met l'image par défaut)
        if self.image:
            return self.image.url
        return self.image_url


# infos uniques du user À PROPOS de l'album
class AlbumUtilisateur(models.Model):
    STATUT_CHOICES = [
        ('possede', _('Possédé')),
        ('souhait', _('Sur ma liste de souhaits')),
    ]

    user = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE, related_name='albums_perso', verbose_name=_("utilisateur"))
    album = models.ForeignKey(Album, on_delete=models.CASCADE, related_name='infos_users', verbose_name=_("album"))
    statut = models.CharField(max_length=10, choices=STATUT_CHOICES, default='souhait', verbose_name=_("statut"))
    note = models.IntegerField(default=0, verbose_name=_("note"))
    isFavori = models.BooleanField(default=False, verbose_name=_("Coup de coeur"))

    class Meta:
        unique_together = ['user', 'album']  # un user peut pas avoir 2 lignes pour le meme album
        verbose_name = _("album d'un utilisateur")
        verbose_name_plural = _("albums des utilisateurs")

    def __str__(self):
        return f"{self.user} — {self.album.titre}"


class Song(models.Model):
    album = models.ForeignKey(Album, on_delete=models.CASCADE, related_name='songs', verbose_name=_("album"))
    titre = models.CharField(max_length=200, verbose_name=_("titre"))
    duree = models.CharField(max_length=10, blank=True, verbose_name=_("durée"))  #X:xx 3:34
    nb = models.IntegerField(verbose_name=_("Numéro"))

    class Meta:
        ordering = ['nb']  # trier song par le numero de tack
        verbose_name = _("chanson")
        verbose_name_plural = _("chansons")

    def __str__(self):
        return f"{self.nb}. {self.titre}"
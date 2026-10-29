from django import forms
import re
from .models import Artiste, Album, AlbumUtilisateur, Song
from datetime import datetime
from django.utils.translation import gettext_lazy as _

class ArtisteForm(forms.ModelForm):
    class Meta:
        model = Artiste
        fields = ['nom', 'pays']
        widgets = {
            'nom': forms.TextInput(attrs={'class': 'form-control'}),
            'pays': forms.TextInput(attrs={'class': 'form-control'}),
        }


class AlbumForm(forms.ModelForm):
    class Meta:
        model = Album
        fields = ['titre', 'artiste', 'genre', 'annee_sortie', 'image', 'image_url']
        
        #respecify forms to add bootstrap classes
        widgets = {
            'titre': forms.TextInput(attrs={'class': 'form-control'}),
            'artiste': forms.Select(attrs={'class': 'form-select'}),
            'genre': forms.Select(attrs={'class': 'form-select'}),
            'annee_sortie': forms.NumberInput(attrs={'class': 'form-control'}),
            'image': forms.ClearableFileInput(attrs={'class': 'form-control'}),
            'image_url': forms.URLInput(attrs={'class': 'form-control', 'placeholder': 'https://...'}),
        }
    def clean_annee_sortie(self):
        annee = self.cleaned_data['annee_sortie']
        if annee < 1900 or annee > datetime.now().year:
            raise forms.ValidationError(_("L'année doit être entre 1900 et celle actuelle."))
        return annee


# form des infos perso du user sur un album (statut, note, coup de coeur)
class AlbumUtilisateurForm(forms.ModelForm):
    class Meta:
        model = AlbumUtilisateur
        fields = ['statut', 'note', 'isFavori']
        widgets = {
            'statut': forms.Select(attrs={'class': 'form-select'}),
            'note': forms.NumberInput(attrs={'class': 'form-control', 'min': 0, 'max': 5}),
            'isFavori': forms.CheckboxInput(attrs={'class': 'form-check-input'}),
        }

    def clean_note(self):
        note = self.cleaned_data['note']
        if note < 0 or note > 5:
            raise forms.ValidationError(_("La note doit être entre 0 et 5."))
        return note

class SongForm(forms.ModelForm):
    class Meta:
        model = Song
        fields = ['titre', 'duree', 'nb']
        widgets = {
            'titre': forms.TextInput(attrs={'class': 'form-control'}),
            'duree': forms.TextInput(attrs={'class': 'form-control', 'placeholder': _('ex: 3:45')}),
            'nb': forms.NumberInput(attrs={'class': 'form-control', 'min': 1}),
        }

    def __init__(self, *args, album=None, **kwargs):
        # l'album n'est pas un champ du form (il vient de l'URL), je le garde en attribut
        # pour aller comparer le nb avec les pistes deja existantes dans clean_nb
        self.album = album
        super().__init__(*args, **kwargs)

    #Methodes clean_X lancées automatiquement par django lorsque is_valid est called
    #================== ERREUR FORMS 

    def clean_duree(self):
        duree = self.cleaned_data['duree']
        if not re.match(r'^\d{1,2}:\d{2}$', duree):
            raise forms.ValidationError(_("Le format doit être m:ss ou mm:ss (ex: 3:45, 12:07)."))
        return duree

    def clean_nb(self):
        nb = self.cleaned_data['nb']
        if self.album is not None:
            dernier = self.album.songs.order_by('-nb').first()
            if dernier is not None and nb <= dernier.nb:
                raise forms.ValidationError(
                    _("Le numéro doit être supérieur au dernier numéro existant (%(dernier)s).") % {'dernier': dernier.nb}
                )
        return nb

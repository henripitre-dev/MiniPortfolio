from modeltranslation.translator import register, TranslationOptions
from .models import Genre, Artiste

@register(Genre)
class GenreTranslationOptions(TranslationOptions):
    fields = ('nom',)
    empty_values = None   # nom est unique : une traduction vide = NULL (pas "" en double)

@register(Artiste)
class ArtisteTranslationOptions(TranslationOptions):
    fields = ('pays',)
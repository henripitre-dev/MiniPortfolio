from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('vinylvault', '0007_alter_album_options_alter_artiste_options_and_more'),
    ]

    operations = [
        migrations.AlterField(
            model_name='album',
            name='image',
            field=models.ImageField(blank=True, default='', upload_to='albums/', verbose_name='image'),
        ),
        migrations.AddField(
            model_name='album',
            name='image_url',
            field=models.URLField(blank=True, verbose_name='URL de la pochette'),
        ),
    ]

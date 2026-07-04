import re

with open("SportTrack-v1.AccesoDatos/SportTrackDbContext.cs", "r", encoding="utf-8") as f:
    content = f.read()

# Replace specific primary keys and foreign keys mapping
replacements = {
    # Keys
    r"modelBuilder\.Entity<Federacion>\(entity => \{\s*entity\.ToTable\(\"Federaciones\", \"federacion\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Federacion>(entity => { \n                entity.ToTable(\"Federaciones\", \"federacion\"); \n                entity.HasKey(e => e.IdFederacion);",
    r"modelBuilder\.Entity<Club>\(entity =>\s*\{\s*entity\.ToTable\(\"Clubes\", \"catalogos\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Club>(entity =>\n            {\n                entity.ToTable(\"Clubes\", \"catalogos\");\n                entity.HasKey(e => e.IdClub);",
    r"modelBuilder\.Entity<Participante>\(entity =>\s*\{\s*entity\.ToTable\(\"Participantes\", \"regatas\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Participante>(entity =>\n            {\n                entity.ToTable(\"Participantes\", \"regatas\");\n                entity.HasKey(e => e.ParticipanteId);",
    r"modelBuilder\.Entity<Usuario>\(entity =>\s*\{\s*entity\.ToTable\(\"Usuarios\", \"seguridad\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Usuario>(entity =>\n            {\n                entity.ToTable(\"Usuarios\", \"seguridad\");\n                entity.HasKey(e => e.IdUsuario);",
    r"modelBuilder\.Entity<EventoPrueba>\(entity =>\s*\{\s*entity\.ToTable\(\"EventoPruebas\", \"regatas\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<EventoPrueba>(entity =>\n            {\n                entity.ToTable(\"EventoPruebas\", \"regatas\");\n                entity.HasKey(e => e.IdEventoPrueba);",
    r"modelBuilder\.Entity<Prueba>\(entity =>\s*\{\s*entity\.ToTable\(\"Pruebas\", \"regatas\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Prueba>(entity =>\n            {\n                entity.ToTable(\"Pruebas\", \"regatas\");\n                entity.HasKey(e => e.IdPrueba);",
    r"modelBuilder\.Entity<Evento>\(entity =>\s*\{\s*entity\.ToTable\(\"Eventos\", \"regatas\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Evento>(entity =>\n            {\n                entity.ToTable(\"Eventos\", \"regatas\");\n                entity.HasKey(e => e.IdEvento);",
    r"modelBuilder\.Entity<Inscripcion>\(entity =>\s*\{\s*entity\.ToTable\(\"Inscripciones\", \"regatas\"\);\s*entity\.HasKey\(e => e\.Id\);": r"modelBuilder.Entity<Inscripcion>(entity =>\n            {\n                entity.ToTable(\"Inscripciones\", \"regatas\");\n                entity.HasKey(e => e.IdInscripcion);",

    # Property mappings
    r"entity\.Property\(e => e\.Id\)": r"// entity.Property(e => e.Id)",
    r"entity\.HasForeignKey\(e => e\.ClubId\)": r"entity.HasForeignKey(e => e.IdClub)",
    r"entity\.HasForeignKey\(e => e\.FederacionId\)": r"entity.HasForeignKey(e => e.IdFederacion)",
    r"entity\.HasForeignKey\(e => e\.ParticipanteId\)": r"entity.HasForeignKey(e => e.ParticipanteId)", # Keep the same
    r"entity\.HasForeignKey\(e => e\.EventoPruebaId\)": r"entity.HasForeignKey(e => e.IdEventoPrueba)",
    r"entity\.HasForeignKey\(e => e\.PruebaId\)": r"entity.HasForeignKey(e => e.IdPrueba)",
    r"entity\.HasForeignKey\(e => e\.BoteId\)": r"entity.HasForeignKey(e => e.TipoBote)",
    r"entity\.HasForeignKey\(e => e\.CategoriaId\)": r"entity.HasForeignKey(e => e.CategoriaEdad)",
    r"entity\.HasForeignKey\(e => e\.SexoId\)": r"entity.HasForeignKey(e => e.SexoCompetencia)",
    r"entity\.HasForeignKey\(e => e\.EventoId\)": r"entity.HasForeignKey(e => e.IdEvento)",
    r"entity\.HasIndex\(e => e\.ClubId\)": r"entity.HasIndex(e => e.IdClub)",
    r"entity\.HasIndex\(e => e\.FederacionId\)": r"entity.HasIndex(e => e.IdFederacion)",
    r"entity\.HasIndex\(e => e\.EventoId\)": r"entity.HasIndex(e => e.IdEvento)",
    r"entity\.HasIndex\(e => e\.PruebaId\)": r"entity.HasIndex(e => e.IdPrueba)",
    
    # Specifics for EventoPrueba Index
    r"e\.EventoId, e\.PruebaId, e\.FechaHora": r"e.IdEvento, e.IdPrueba, e.FechaHora",
    r"e\.BoteId, e\.CategoriaId, e\.DistanciaId, e\.SexoId": r"e.TipoBote, e.CategoriaEdad, e.DistanciaId, e.SexoCompetencia"
}

for old, new in replacements.items():
    content = re.sub(old, new, content, flags=re.MULTILINE)

with open("SportTrack-v1.AccesoDatos/SportTrackDbContext.cs", "w", encoding="utf-8") as f:
    f.write(content)
print("Updated DbContext.")

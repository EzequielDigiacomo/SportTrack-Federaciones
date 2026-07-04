import re

with open("SportTrack-v1.AccesoDatos/SportTrackDbContext.cs", "r", encoding="utf-8") as f:
    content = f.read()

replacements = {
    # Keys
    "entity.HasKey(e => e.Id);": "entity.HasKey(e => e.Id);", # Default, but we will target specific entities
    "modelBuilder.Entity<Club>(entity =>\n            {\n                entity.ToTable(\"Clubes\", \"catalogos\");\n\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<Club>(entity =>\n            {\n                entity.ToTable(\"Clubes\", \"catalogos\");\n\n                entity.HasKey(e => e.IdClub);",
    "modelBuilder.Entity<Participante>(entity =>\n            {\n                entity.ToTable(\"Participantes\", \"regatas\");\n\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<Participante>(entity =>\n            {\n                entity.ToTable(\"Participantes\", \"regatas\");\n\n                entity.HasKey(e => e.ParticipanteId);",
    "modelBuilder.Entity<Usuario>(entity =>\n            {\n                entity.ToTable(\"Usuarios\", \"seguridad\");\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<Usuario>(entity =>\n            {\n                entity.ToTable(\"Usuarios\", \"seguridad\");\n                entity.HasKey(e => e.IdUsuario);",
    "modelBuilder.Entity<EventoPrueba>(entity =>\n            {\n                entity.ToTable(\"EventoPruebas\", \"regatas\");\n\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<EventoPrueba>(entity =>\n            {\n                entity.ToTable(\"EventoPruebas\", \"regatas\");\n\n                entity.HasKey(e => e.IdEventoPrueba);",
    "modelBuilder.Entity<Prueba>(entity =>\n            {\n                entity.ToTable(\"Pruebas\", \"regatas\");\n\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<Prueba>(entity =>\n            {\n                entity.ToTable(\"Pruebas\", \"regatas\");\n\n                entity.HasKey(e => e.IdPrueba);",
    "modelBuilder.Entity<Evento>(entity =>\n            {\n                entity.ToTable(\"Eventos\", \"regatas\");\n\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<Evento>(entity =>\n            {\n                entity.ToTable(\"Eventos\", \"regatas\");\n\n                entity.HasKey(e => e.IdEvento);",
    "modelBuilder.Entity<Inscripcion>(entity =>\n            {\n                entity.ToTable(\"Inscripciones\", \"regatas\");\n                entity.HasKey(e => e.Id);": "modelBuilder.Entity<Inscripcion>(entity =>\n            {\n                entity.ToTable(\"Inscripciones\", \"regatas\");\n                entity.HasKey(e => e.IdInscripcion);",

    # Property mappings
    "entity.Property(e => e.Id)\n                    .ValueGeneratedOnAdd();": "// property removed",
    "entity.HasForeignKey(e => e.ClubId)": "entity.HasForeignKey(e => e.IdClub)",
    "entity.HasForeignKey(e => e.FederacionId)": "entity.HasForeignKey(e => e.IdFederacion)",
    "entity.HasForeignKey(e => e.EventoPruebaId)": "entity.HasForeignKey(e => e.IdEventoPrueba)",
    "entity.HasForeignKey(e => e.PruebaId)": "entity.HasForeignKey(e => e.IdPrueba)",
    "entity.HasForeignKey(e => e.BoteId)": "entity.HasForeignKey(e => e.TipoBote)",
    "entity.HasForeignKey(e => e.CategoriaId)": "entity.HasForeignKey(e => e.CategoriaEdad)",
    "entity.HasForeignKey(e => e.SexoId)": "entity.HasForeignKey(e => e.SexoCompetencia)",
    "entity.HasForeignKey(e => e.EventoId)": "entity.HasForeignKey(e => e.IdEvento)",
    "entity.HasIndex(e => e.ClubId)": "entity.HasIndex(e => e.IdClub)",
    "entity.HasIndex(e => e.FederacionId)": "entity.HasIndex(e => e.IdFederacion)",
    "entity.HasIndex(e => e.EventoId)": "entity.HasIndex(e => e.IdEvento)",
    "entity.HasIndex(e => e.PruebaId)": "entity.HasIndex(e => e.IdPrueba)",
    
    # Specifics for EventoPrueba Index
    "e.EventoId, e.PruebaId, e.FechaHora": "e.IdEvento, e.IdPrueba, e.FechaHora",
    "e.BoteId, e.CategoriaId, e.DistanciaId, e.SexoId": "e.TipoBote, e.CategoriaEdad, e.DistanciaId, e.SexoCompetencia"
}

for old, new in replacements.items():
    content = content.replace(old, new)

with open("SportTrack-v1.AccesoDatos/SportTrackDbContext.cs", "w", encoding="utf-8") as f:
    f.write(content)
print("Updated DbContext safely.")

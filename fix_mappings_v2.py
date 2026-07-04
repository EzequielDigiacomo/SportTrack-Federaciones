import re
import os

mapping = {
    # Club mappings
    "Club.IdClub": "Id",
    "Club.Siglas": "Sigla",
    "Club.IdFederacion": "FederacionId",
    "Club.EstadoMatricula": "EstadoMatriculaId",
    
    # Participante mappings
    "Participante.ParticipanteId": "Id",
    "Participante.Documento": "Dni",
    "Participante.Usuario": "Id",
    
    # AtletaFederado mappings
    "AtletaFederado.ParticipanteId": "IdParticipante",
    "AtletaFederado.IdFederacion": "FederacionId",
    "AtletaFederado.Id": "IdParticipante",
    
    # Tutor mappings
    "Tutor.ParticipanteId": "IdParticipante",
    "Tutor.Id": "IdParticipante",
    "TutorCreateDto.Id": "ParticipanteId",
    
    # Entrenador mappings
    "Entrenador.ParticipanteId": "IdParticipante",
    "Entrenador.Id": "IdParticipante",
    "Entrenador.IdFederacion": "FederacionId",
    "EntrenadorCreateDto.Id": "ParticipanteId",
    
    # DelegadoClub mappings
    "DelegadoClub.ParticipanteId": "IdParticipante",
    "DelegadoClub.Id": "IdParticipante",
    "DelegadoClub.IdFederacion": "FederacionId",
    "DelegadoClubCreateDto.Id": "ParticipanteId",
    
    # AtletaTutor mappings
    "AtletaTutor.ParticipanteId": "IdParticipante",
    
    # Usuario mappings
    "Usuario.IdUsuario": "Id",
    "Usuario.EstaActivo": "Activo",
    "Usuario.UltimoAcceso": "FechaCreacion",
    "Usuario.UltimoLogin": "FechaCreacion",
    "Usuario.IdFederacion": "FederacionId",
    "Usuario.IdClub": "ClubId",
    
    # EventoPrueba mappings
    "EventoPrueba.IdEventoPrueba": "Id",
    "EventoPrueba.IdEvento": "EventoId",
    "EventoPrueba.IdPrueba": "PruebaId",
    "EventoPrueba.PrecioCategoria": "Precio",
    
    # Prueba mappings
    "Prueba.IdPrueba": "Id",
    "Prueba.TipoBote": "BoteId",
    "Prueba.CategoriaEdad": "CategoriaId",
    "Prueba.SexoCompetencia": "SexoId",
    
    # Evento mappings
    "Evento.IdEvento": "Id",
    "Evento.IdFederacion": "FederacionId",
    "Evento.IdClub": "ClubId",
    "Evento.EstaActivo": "Activo",
    "Evento.Activo": "Activo",
    "Evento.Descripcion": "Nombre",
    "Evento.Observaciones": "Nombre",
    "Evento.Pruebas": "EventoPruebas",
    "Evento.Inscripciones": "EventoPruebas",
    "Evento.FechaInicioInscripciones": "FechaInicio",
    "Evento.FechaInicio": "FechaInicio",
    
    # Inscripcion mappings
    "Inscripcion.IdInscripcion": "Id",
    "Inscripcion.IdEventoPrueba": "EventoPruebaId",
    "Inscripcion.AtletaFederado": "Participante",
    "InscripcionCreateDto.Id": "IdInscripcion",
    "InscripcionCreateDto.EventoPruebaId": "IdEventoPrueba",
    
    # Federacion mappings
    "Federacion.IdFederacion": "Id",
    
    # PagoTransaccion mappings
    "PagoTransaccion.ParticipanteId": "ParticipanteId",
    "PagoTransaccionCreateDto.Id": "IdPago",
    
    # Persona mappings
    "PersonaCreateDto.Dni": "Documento",
    "AtletaCreateDto.Id": "ParticipanteId"
}

log_file = "build_errors.txt"
regex = re.compile(r'^(.*?)\((\d+),(\d+)\): error CS1061: "(.*?)" no contiene una definición para "(.*?)"')

fixes = {}

with open(log_file, "r", encoding="utf-16") as f:
    for line in f:
        match = regex.search(line)
        if match:
            file_path = match.group(1).strip()
            line_num = int(match.group(2))
            class_name = match.group(4)
            prop_name = match.group(5)
            
            key = f"{class_name}.{prop_name}"
            if key in mapping:
                if file_path not in fixes:
                    fixes[file_path] = {}
                # Keep replacing the line with any matching property
                if line_num not in fixes[file_path]:
                    fixes[file_path][line_num] = []
                fixes[file_path][line_num].append((prop_name, mapping[key]))

for file_path, line_fixes in fixes.items():
    if not os.path.exists(file_path):
        continue
    
    with open(file_path, "r", encoding="latin-1") as f:
        lines = f.readlines()
        
    for line_num, replacements in line_fixes.items():
        idx = line_num - 1
        for old_prop, new_prop in replacements:
            # Safely replace only as a word boundary
            lines[idx] = re.sub(r'\b' + old_prop + r'\b', new_prop, lines[idx])
        
    with open(file_path, "w", encoding="latin-1") as f:
        f.writelines(lines)

print(f"Applied fixes to {len(fixes)} files.")

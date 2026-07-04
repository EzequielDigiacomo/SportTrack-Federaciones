import re
import os

mapping = {
    "Club.IdClub": "Id",
    "Club.Siglas": "Sigla",
    "Participante.ParticipanteId": "Id",
    "AtletaFederado.ParticipanteId": "Id",
    "Tutor.ParticipanteId": "Id",
    "Entrenador.ParticipanteId": "Id",
    "DelegadoClub.ParticipanteId": "Id",
    "Usuario.IdUsuario": "Id",
    "Usuario.EstaActivo": "Activo",
    "Usuario.UltimoAcceso": "UltimoLogin",
    "Usuario.IdFederacion": "FederacionId",
    "Usuario.IdClub": "ClubId",
    "EventoPrueba.IdEventoPrueba": "Id",
    "EventoPrueba.IdEvento": "EventoId",
    "EventoPrueba.IdPrueba": "PruebaId",
    "EventoPrueba.PrecioCategoria": "Precio",
    "Prueba.IdPrueba": "Id",
    "Evento.IdEvento": "Id",
    "Evento.IdFederacion": "FederacionId",
    "Evento.IdClub": "ClubId",
    "Evento.EstaActivo": "Activo",
    "Evento.Descripcion": "Observaciones",
    "Inscripcion.IdInscripcion": "Id",
    "Inscripcion.IdEventoPrueba": "EventoPruebaId",
    "Federacion.IdFederacion": "Id",
    "Club.IdFederacion": "FederacionId",
    "Entrenador.IdFederacion": "FederacionId",
    "DelegadoClub.IdFederacion": "FederacionId",
    "AtletaFederado.IdFederacion": "FederacionId",
    "Club.EstadoMatricula": "EstadoMatriculaId",
    "PagoTransaccion.ParticipanteId": "ParticipanteId",
    "PersonaCreateDto.Dni": "Documento"
}

log_file = "build_errors_v13.txt"

regex = re.compile(r'^(.*?)\((\d+),(\d+)\): error CS1061: "(.*?)" no contiene una definición para "(.*?)"')

fixes = {}

with open(log_file, "r", encoding="utf-16") as f:
    for line in f:
        match = regex.search(line)
        if match:
            file_path = match.group(1).strip()
            line_num = int(match.group(2))
            col_num = int(match.group(3))
            class_name = match.group(4)
            prop_name = match.group(5)
            
            key = f"{class_name}.{prop_name}"
            if key in mapping:
                if file_path not in fixes:
                    fixes[file_path] = {}
                # We store the replacement for this line
                fixes[file_path][line_num] = (prop_name, mapping[key])

for file_path, line_fixes in fixes.items():
    if not os.path.exists(file_path):
        continue
    
    with open(file_path, "r", encoding="latin-1") as f:
        lines = f.readlines()
        
    for line_num, (old_prop, new_prop) in line_fixes.items():
        idx = line_num - 1
        # Simple replace on that line. Might replace other occurrences on the same line, but usually safe.
        lines[idx] = lines[idx].replace(old_prop, new_prop)
        
    with open(file_path, "w", encoding="latin-1") as f:
        f.writelines(lines)

print(f"Applied fixes to {len(fixes)} files.")

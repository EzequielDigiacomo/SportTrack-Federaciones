using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string dir = @"SportTrack-v1.Controladores\Federaciones";
        
        string[] files = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string content = File.ReadAllText(file);
            bool changed = false;
            
            // Map Entity properties properly (ONLY right-hand side or left-hand side depending on usage)
            
            // AtletaTutor.IdParticipante -> AtletaTutor.IdTutor (or AtletaFederado? The errors were about AtletaTutor)
            if (content.Contains("AtletaTutor")) {
                content = content.Replace(".IdParticipante", ".IdTutor");
                changed = true;
            }
            
            // Participante.IdParticipante -> Participante.Id
            if (content.Contains("Participante")) {
                // regex replace Participante.IdParticipante
                content = Regex.Replace(content, @"\b(participante|Participante)\.IdParticipante\b", "$1.Id");
                content = Regex.Replace(content, @"\b(participante|Participante)\.Documento\b", "$1.Dni");
                changed = true;
            }
            
            if (content.Contains("Club")) {
                content = Regex.Replace(content, @"\b(club|c|Club)\.IdClub\b", "$1.Id");
                content = Regex.Replace(content, @"\b(club|c|Club)\.Siglas\b", "$1.Sigla");
                content = Regex.Replace(content, @"\b(club|c|Club)\.IdFederacion\b", "$1.FederacionId");
                content = Regex.Replace(content, @"\b(club|c|Club)\.EstadoMatricula\b", "$1.Activo");
                // Remove includes for properties that don't exist
                content = Regex.Replace(content, @"\.Include\(c => c\.AtletasFederados\)", "");
                content = Regex.Replace(content, @"\.Include\(c => c\.Entrenadores\)", "");
                content = Regex.Replace(content, @"\.Include\(c => c\.Representantes\)", "");
                content = Regex.Replace(content, @"\.Include\(c => c\.Pagos\)", "");
                changed = true;
            }
            
            if (changed)
                File.WriteAllText(file, content);
        }
    }
}

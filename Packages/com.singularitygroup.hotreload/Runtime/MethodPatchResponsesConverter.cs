#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Newtonsoft.Json;

namespace SingularityGroup.HotReload.JsonConverters {
    internal class MethodPatchResponsesConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(List<MethodPatchResponse>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var list = new List<MethodPatchResponse>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    list.Add(ReadMethodPatchResponse(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SMethod list
                }
            }

            return list;
        }
        
        private MethodPatchResponse ReadMethodPatchResponse(JsonReader reader) {
            string id = null;
            CodePatch[] patches = null;
            string[] failures = null;
            SMethod[] removedMethod = null;
            SField[] alteredFields = null;
            SField[] addedFieldInitializerFields = null;
            SMethod[] addedFieldInitializerInitializers = null;
            SField[] removedFieldInitializers = null;
            SField[] newFieldDefinitions = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(MethodPatchResponse.id):
                        id = reader.ReadAsString();
                        break;

                    case nameof(MethodPatchResponse.patches):
                        patches = ReadPatches(reader);
                        break;

                    case nameof(MethodPatchResponse.failures):
                        failures = ReadStringArray(reader);
                        break;

                    case nameof(MethodPatchResponse.removedMethod):
                        removedMethod = ReadSMethodArray(reader);
                        break;
                    
                    case nameof(MethodPatchResponse.alteredFields):
                        alteredFields = ReadSFields(reader);
                        break;

                    case nameof(MethodPatchResponse.addedFieldInitializerFields):
                        addedFieldInitializerFields = ReadSFields(reader);
                        break;

                    case nameof(MethodPatchResponse.addedFieldInitializerInitializers):
                        addedFieldInitializerInitializers = ReadSMethodArray(reader);
                        break;

                    case nameof(MethodPatchResponse.removedFieldInitializers):
                        removedFieldInitializers = ReadSFields(reader);
                        break;
                        
                    case nameof(MethodPatchResponse.newFieldDefinitions):
                        newFieldDefinitions = ReadSFields(reader);
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }
            
            return new MethodPatchResponse(
                id ?? string.Empty,
                patches ?? Array.Empty<CodePatch>(), 
                failures ?? Array.Empty<string>(), 
                removedMethod ?? Array.Empty<SMethod>(),
                alteredFields ?? Array.Empty<SField>(),
                // Note: suggestions don't have to be persisted here 
                Array.Empty<PartiallySupportedChange>(),
                Array.Empty<HotReloadSuggestionKind>(),
                addedFieldInitializerFields ?? Array.Empty<SField>(),
                addedFieldInitializerInitializers ?? Array.Empty<SMethod>(),
                removedFieldInitializers ?? Array.Empty<SField>(),
                newFieldDefinitions ?? Array.Empty<SField>()
            );
        }

        private CodePatch[] ReadPatches(JsonReader reader) {
            var patches = new List<CodePatch>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndArray) {
                    break;
                }
                if (reader.TokenType != JsonToken.StartObject) {
                    continue;
                }
                string patchId = null;
                string assemblyName = null;
                byte[] patchAssembly = null;
                byte[] patchPdb = null;
                SMethod[] modifiedMethods = null;
                SMethod[] patchMethods = null;
                SMethod[] newMethods = null;
                SUnityJob[] unityJobs = null;
                SField[] newFields = null;
                SField[] deletedFields = null;
                SField[] renamedFieldsFrom = null;
                SField[] renamedFieldsTo = null;
                SField[] propertyAttributesFieldOriginal = null;
                SField[] propertyAttributesFieldUpdated = null;

                while (reader.Read()) {
                    if (reader.TokenType == JsonToken.EndObject) {
                        break;
                    }
                    if (reader.TokenType != JsonToken.PropertyName) {
                        continue;
                    }
                    var propertyName = (string)reader.Value;

                    switch (propertyName) {
                        case nameof(CodePatch.patchId):
                            patchId = reader.ReadAsString();
                            break;

                        case nameof(CodePatch.assemblyName):
                            assemblyName = reader.ReadAsString();
                            break;
                        
                        case nameof(CodePatch.patchAssembly):
                            patchAssembly = Convert.FromBase64String(reader.ReadAsString());
                            break;
                        
                        case nameof(CodePatch.patchPdb):
                            patchPdb = Convert.FromBase64String(reader.ReadAsString());
                            break;
                        
                        case nameof(CodePatch.modifiedMethods):
                            modifiedMethods = ReadSMethodArray(reader);
                            break;
                        
                        case nameof(CodePatch.patchMethods):
                            patchMethods = ReadSMethodArray(reader);
                            break;
                        
                        case nameof(CodePatch.newMethods):
                            newMethods = ReadSMethodArray(reader);
                            break;
                        
                        case nameof(CodePatch.unityJobs):
                            unityJobs = ReadSUnityJobArray(reader);
                            break;
                        
                        case nameof(CodePatch.newFields):
                            newFields = ReadSFields(reader);
                            break;
                        case nameof(CodePatch.deletedFields):
                            deletedFields = ReadSFields(reader);
                            break;
                        
                        case nameof(CodePatch.renamedFieldsFrom):
                            renamedFieldsFrom = ReadSFields(reader);
                            break;
                        
                        case nameof(CodePatch.renamedFieldsTo):
                            renamedFieldsTo = ReadSFields(reader);
                            break;
                        
                        case nameof(CodePatch.propertyAttributesFieldOriginal):
                            propertyAttributesFieldOriginal = ReadSFields(reader);
                            break;
                        
                        case nameof(CodePatch.propertyAttributesFieldUpdated):
                            propertyAttributesFieldUpdated = ReadSFields(reader);
                            break;

                        default:
                            reader.Skip(); // Skip unknown properties
                            break;
                    }
                }

                patches.Add(new CodePatch(
                    patchId: patchId ?? string.Empty,
                    assemblyName: assemblyName ?? string.Empty,
                    patchAssembly: patchAssembly ?? Array.Empty<byte>(),
                    patchPdb: patchPdb ?? Array.Empty<byte>(),
                    modifiedMethods: modifiedMethods ?? Array.Empty<SMethod>(),
                    patchMethods: patchMethods ?? Array.Empty<SMethod>(),
                    newMethods: newMethods ?? Array.Empty<SMethod>(),
                    unityJobs: unityJobs ?? Array.Empty<SUnityJob>(),
                    newFields: newFields ?? Array.Empty<SField>(),
                    deletedFields: deletedFields ?? Array.Empty<SField>(),
                    renamedFieldsFrom: renamedFieldsFrom ?? Array.Empty<SField>(),
                    renamedFieldsTo: renamedFieldsTo ?? Array.Empty<SField>(),
                    propertyAttributesFieldOriginal: propertyAttributesFieldOriginal ?? Array.Empty<SField>(),
                    propertyAttributesFieldUpdated: propertyAttributesFieldUpdated ?? Array.Empty<SField>()
                ));
            }

            return patches.ToArray();
        }

        private string[] ReadStringArray(JsonReader reader) {
            var list = new List<string>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.String) {
                    list.Add((string)reader.Value);
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the string list
                }
            }

            return list.ToArray();
        }

        private SMethod[] ReadSMethodArray(JsonReader reader) {
            var list = new List<SMethod>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    list.Add(ReadSMethod(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SMethod list
                }
            }

            return list.ToArray();
        }

        private SType[] ReadSTypeArray(JsonReader reader) {
            var list = new List<SType>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    list.Add(ReadSType(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SType list
                }
            }

            return list.ToArray();
        }
        
        private SUnityJob[] ReadSUnityJobArray(JsonReader reader) {
            var array = new List<SUnityJob>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    array.Add(ReadSUnityJob(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SUnityJob array
                }
            }

            return array.ToArray();
        }
        
        private SField[] ReadSFields(JsonReader reader) {
            var array = new List<SField>();
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.StartObject) {
                    array.Add(ReadSField(reader));
                } else if (reader.TokenType == JsonToken.EndArray) {
                    break; // End of the SUnityJob array
                }
            }
            return array.ToArray();
        }

        private SMethod ReadSMethod(JsonReader reader) {
            string assemblyName = null;
            string displayName = null;
            int metadataToken = default(int);
            string simpleName = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SMethod.assemblyName):
                        assemblyName = reader.ReadAsString();
                        break;

                    case nameof(SMethod.displayName):
                        displayName = reader.ReadAsString();
                        break;
                    
                    case nameof(SMethod.metadataToken):
                        metadataToken = reader.ReadAsInt32() ?? default(int);
                        break;
                    
                    case nameof(SMethod.simpleName):
                        simpleName = reader.ReadAsString();
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SMethod(
                assemblyName ?? string.Empty,
                displayName ?? string.Empty,
                metadataToken, 
                simpleName ?? string.Empty
            );
        }

        private SType ReadSType(JsonReader reader) {
            string assemblyName = null;
            string typeName = null;
            int? metadataToken = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.Null) {
                    return null;
                }
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SType.assemblyName):
                        assemblyName = reader.ReadAsString();
                        break;

                    case nameof(SType.typeName):
                        typeName = reader.ReadAsString();
                        break;

                    case nameof(SType.metadataToken):
                        metadataToken = reader.ReadAsInt32();
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SType(
                assemblyName ?? string.Empty,
                typeName ?? string.Empty,
                metadataToken ?? 0
            );
        }

        private SUnityJob ReadSUnityJob(JsonReader reader) {
            int metadataToken = default(int);
            UnityJobKind jobKind = default(UnityJobKind);

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SUnityJob.metadataToken):
                        metadataToken = reader.ReadAsInt32() ?? 0;
                        break;

                    case nameof(SUnityJob.jobKind):
                        var jobKindStr = reader.ReadAsString();
                        Enum.TryParse(jobKindStr, out jobKind);
                        break;

                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SUnityJob(metadataToken, jobKind);
        }
        
        private SField ReadSField(JsonReader reader) {
            SType declaringType = null;
            string fieldName = null;
            string assemblyName = null;
            int? metadataToken = null;
            bool? serializable = null;
            bool? isStatic = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                if (reader.TokenType != JsonToken.PropertyName) {
                    continue;
                }
                var propertyName = (string)reader.Value;

                switch (propertyName) {
                    case nameof(SField.declaringType):
                        declaringType = ReadSType(reader);
                        break;
                    
                    case nameof(SField.fieldName):
                        fieldName = reader.ReadAsString();
                        break;

                    case nameof(SField.assemblyName):
                        assemblyName = reader.ReadAsString();
                        break;
                    
                    case nameof(SField.metadataToken):
                        metadataToken = reader.ReadAsInt32();
                        break;

                    case nameof(SField.serializable):
                        serializable = reader.ReadAsBoolean();
                        break;
                    
                    case nameof(SField.isStatic):
                        isStatic = reader.ReadAsBoolean();
                        break;
                    
                    default:
                        reader.Skip(); // Skip unknown properties
                        break;
                }
            }

            return new SField(declaringType: declaringType, fieldName: fieldName, assemblyName: assemblyName, metadataToken ?? 0, isStatic ?? false, serializable ?? false);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var responses = (List<MethodPatchResponse>)value;
            if (responses == null) {
                writer.WriteNull();
                return;
            }
            
            writer.WriteStartArray();
            foreach (var response in responses) {
                writer.WriteStartObject();
                
                writer.WritePropertyName(nameof(response.id));
                writer.WriteValue(response.id);

                if (response.patches != null) {
                    writer.WritePropertyName(nameof(response.patches));
                    writer.WriteStartArray();
                    foreach (var responsePatch in response.patches) {
                        writer.WriteStartObject();
                        
                        writer.WritePropertyName(nameof(responsePatch.patchId));
                        writer.WriteValue(responsePatch.patchId);
                        writer.WritePropertyName(nameof(responsePatch.assemblyName));
                        writer.WriteValue(responsePatch.assemblyName);
                        writer.WritePropertyName(nameof(responsePatch.patchAssembly));
                        writer.WriteValue(Convert.ToBase64String(responsePatch.patchAssembly));
                        writer.WritePropertyName(nameof(responsePatch.patchPdb));
                        writer.WriteValue(Convert.ToBase64String(responsePatch.patchPdb));

                        if (responsePatch.modifiedMethods != null) {
                            writer.WritePropertyName(nameof(responsePatch.modifiedMethods));
                            writer.WriteStartArray();
                            foreach (var modifiedMethod in responsePatch.modifiedMethods) {
                                WriteSMethod(writer, modifiedMethod);
                            }
                            writer.WriteEndArray();
                        }

                        if (responsePatch.patchMethods != null) {
                            writer.WritePropertyName(nameof(responsePatch.patchMethods));
                            writer.WriteStartArray();
                            foreach (var patchMethod in responsePatch.patchMethods) {
                                WriteSMethod(writer, patchMethod);
                            }
                            writer.WriteEndArray();
                        }

                        if (responsePatch.newMethods != null) {
                            writer.WritePropertyName(nameof(responsePatch.newMethods));
                            writer.WriteStartArray();
                            foreach (var newMethod in responsePatch.newMethods) {
                                WriteSMethod(writer, newMethod);
                            }
                            writer.WriteEndArray();
                        }

                        if (responsePatch.unityJobs != null) {
                            writer.WritePropertyName(nameof(responsePatch.unityJobs));
                            writer.WriteStartArray();
                            foreach (var unityJob in responsePatch.unityJobs) {
                                writer.WriteStartObject();

                                writer.WritePropertyName(nameof(unityJob.metadataToken));
                                writer.WriteValue(unityJob.metadataToken);
                                writer.WritePropertyName(nameof(unityJob.jobKind));
                                writer.WriteValue(unityJob.jobKind.ToString());

                                writer.WriteEndObject();
                            }
                            writer.WriteEndArray();
                        }
                        
                        if (responsePatch.newFields != null) {
                            writer.WritePropertyName(nameof(responsePatch.newFields));
                            writer.WriteStartArray();
                            foreach (var newField in responsePatch.newFields) {
                                WriteSField(writer, newField);
                            }
                            writer.WriteEndArray();
                        }
                        
                        if (responsePatch.deletedFields != null) {
                            writer.WritePropertyName(nameof(responsePatch.deletedFields));
                            writer.WriteStartArray();
                            foreach (var deletedField in responsePatch.deletedFields) {
                                WriteSField(writer, deletedField);
                            }
                            writer.WriteEndArray();
                        }
                        
                        if (responsePatch.renamedFieldsFrom != null) {
                            writer.WritePropertyName(nameof(responsePatch.renamedFieldsFrom));
                            writer.WriteStartArray();
                            foreach (var removedFieldFrom in responsePatch.renamedFieldsFrom) {
                                WriteSField(writer, removedFieldFrom);
                            }
                            writer.WriteEndArray();
                        }
                        
                        if (responsePatch.renamedFieldsTo != null) {
                            writer.WritePropertyName(nameof(responsePatch.renamedFieldsTo));
                            writer.WriteStartArray();
                            foreach (var removedFieldTo in responsePatch.renamedFieldsTo) {
                                WriteSField(writer, removedFieldTo);
                            }
                            writer.WriteEndArray();
                        }
                        
                        if (responsePatch.propertyAttributesFieldOriginal != null) {
                            writer.WritePropertyName(nameof(responsePatch.propertyAttributesFieldOriginal));
                            writer.WriteStartArray();
                            foreach (var removedFieldFrom in responsePatch.propertyAttributesFieldOriginal) {
                                WriteSField(writer, removedFieldFrom);
                            }
                            writer.WriteEndArray();
                        }
                        
                        if (responsePatch.propertyAttributesFieldUpdated != null) {
                            writer.WritePropertyName(nameof(responsePatch.propertyAttributesFieldUpdated));
                            writer.WriteStartArray();
                            foreach (var removedFieldTo in responsePatch.propertyAttributesFieldUpdated) {
                                WriteSField(writer, removedFieldTo);
                            }
                            writer.WriteEndArray();
                        }

                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }

                if (response.failures != null) {
                    writer.WritePropertyName(nameof(response.failures));
                    writer.WriteStartArray();
                    foreach (var failure in response.failures) {
                        writer.WriteValue(failure);
                    }
                    writer.WriteEndArray();
                }

                if (response.removedMethod != null) {
                    writer.WritePropertyName(nameof(response.removedMethod));
                    writer.WriteStartArray();
                    foreach (var removedMethod in response.removedMethod) {
                        WriteSMethod(writer, removedMethod);
                    }
                    writer.WriteEndArray();
                }
                
                if (response.alteredFields != null) {
                    writer.WritePropertyName(nameof(response.alteredFields));
                    writer.WriteStartArray();
                    foreach (var alteredField in response.alteredFields) {
                        WriteSField(writer, alteredField);
                    }
                    writer.WriteEndArray();
                }
                
                if (response.addedFieldInitializerFields != null) {
                    writer.WritePropertyName(nameof(response.addedFieldInitializerFields));
                    writer.WriteStartArray();
                    foreach (var addedFieldInitializerField in response.addedFieldInitializerFields) {
                        WriteSField(writer, addedFieldInitializerField);
                    }
                    writer.WriteEndArray();
                }

                if (response.addedFieldInitializerInitializers != null) {
                    writer.WritePropertyName(nameof(response.addedFieldInitializerInitializers));
                    writer.WriteStartArray();
                    foreach (var addedFieldInitializerInitializer in response.addedFieldInitializerInitializers) {
                        WriteSMethod(writer, addedFieldInitializerInitializer);
                    }
                    writer.WriteEndArray();
                }
                
                if (response.removedFieldInitializers != null) {
                    writer.WritePropertyName(nameof(response.removedFieldInitializers));
                    writer.WriteStartArray();
                    foreach (var removedFieldInitializer in response.removedFieldInitializers) {
                        WriteSField(writer, removedFieldInitializer);
                    }
                    writer.WriteEndArray();
                }
                
                if (response.newFieldDefinitions != null) {
                    writer.WritePropertyName(nameof(response.newFieldDefinitions));
                    writer.WriteStartArray();
                    foreach (var newFieldDefinition in response.newFieldDefinitions) {
                        WriteSField(writer, newFieldDefinition);
                    }
                    writer.WriteEndArray();
                }
                
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        
        void WriteSMethod(JsonWriter writer, SMethod method) {
            writer.WriteStartObject();
            
            writer.WritePropertyName(nameof(method.assemblyName));
            writer.WriteValue(method.assemblyName);
            writer.WritePropertyName(nameof(method.displayName));
            writer.WriteValue(method.displayName);
            writer.WritePropertyName(nameof(method.metadataToken));
            writer.WriteValue(method.metadataToken);
            writer.WritePropertyName(nameof(method.simpleName));
            writer.WriteValue(method.simpleName);
            
            writer.WriteEndObject();
        }
        
        void WriteSField(JsonWriter writer, SField field) {
            writer.WriteStartObject();
            
            writer.WritePropertyName(nameof(field.declaringType));
            writer.WriteSType(field.declaringType);
            writer.WritePropertyName(nameof(field.fieldName));
            writer.WriteValue(field.fieldName);
            writer.WritePropertyName(nameof(field.assemblyName));
            writer.WriteValue(field.assemblyName);
            writer.WritePropertyName(nameof(field.metadataToken));
            writer.WriteValue(field.metadataToken);
            writer.WritePropertyName(nameof(field.serializable));
            writer.WriteValue(field.serializable);
            
            writer.WriteEndObject();
        }

    }
    internal static class MethodPatchResponsesConverterExtensions {
        public static void WriteSType(this JsonWriter writer, SType type) {
            if (type == null) {
                writer.WriteNull();
                return;
            }
            writer.WriteStartObject();
            
            writer.WritePropertyName(nameof(type.assemblyName));
            writer.WriteValue(type.assemblyName);
            writer.WritePropertyName(nameof(type.typeName));
            writer.WriteValue(type.typeName);
            writer.WritePropertyName(nameof(type.metadataToken));
            writer.WriteValue(type.metadataToken);
            
            writer.WriteEndObject();
        }
    }

}
#endif

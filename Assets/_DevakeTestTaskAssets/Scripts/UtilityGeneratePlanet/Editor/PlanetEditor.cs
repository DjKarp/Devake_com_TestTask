using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// <summary>
/// Класс эдитора юнити, что создаёт кнопку для применения изменений внесённых в настройки генерации.
/// Если не включена опция автоматического применения изменений.
/// </summary>
[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{

    private Planet planet;

    private Editor shapeEditor;
    private Editor colourEditor;

    public override void OnInspectorGUI()
    {

        using (var check = new EditorGUI.ChangeCheckScope())
        {

            base.OnInspectorGUI();

            if (check.changed)
            {

                planet.GeneratePlanet();

            }

        }

        if (GUILayout.Button("Generate Planet"))
        {

            planet.GeneratePlanet();

        }

        DrawSettingsEditor(planet.m_ShapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldOut, ref shapeEditor);
        DrawSettingsEditor(planet.m_ColourSettings, planet.OnColourSettingsUpdated, ref planet.colorSettingsFoldOut, ref colourEditor);

    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldOut, ref Editor editor)
    {

        if (settings != null)
        {

            foldOut = EditorGUILayout.InspectorTitlebar(foldOut, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {

                if (foldOut)
                {

                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {

                        if (onSettingsUpdated != null)
                        {

                            onSettingsUpdated();

                        }

                    }

                }

            }
        }

    }

    private void OnEnable()
    {

        planet = (Planet)target;

    }

}

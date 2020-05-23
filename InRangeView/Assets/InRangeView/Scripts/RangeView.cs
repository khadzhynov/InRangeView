using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GG.Utils.InRangeView
{

    public struct AreaPointData
    {
        public Vector3 Position;
        public float Distance;
    }

    public class RangeView : MonoBehaviour
    {
        [SerializeField, Range(0.05f, 0.5f)]
        private float _cellSize = 0.2f;

        [SerializeField]
        private ParticleSystem _particleSystem;

        private Unit _currentUnit;

        [SerializeField]
        private int _currentNumberOfParticles = 0;

        [SerializeField]
        private int _currentNumberOfNavMeshPoints = 0;

        private float _lastUpdateTime;
        private float _minimalUpdateInterval = 0.05f;

        private void OnEnable()
        {
            Unit.OnSelectUnit += OnSelectUnitHandler;

        }

        private void OnDisable()
        {
            Unit.OnSelectUnit -= OnSelectUnitHandler;
        }

        private void OnSelectUnitHandler(Unit unit)
        {
            if (_currentUnit != null)
            {
                _currentUnit.OnPositionUpdate -= OnUpdateMovementHandler;
            }

            _currentUnit = unit;

            _currentUnit.OnPositionUpdate += OnUpdateMovementHandler;

            HighlightMovingArea(unit.transform.position, unit.Range);
        }

        private void OnUpdateMovementHandler()
        {
            if (Time.realtimeSinceStartup - _lastUpdateTime >= _minimalUpdateInterval)
            {
                HighlightMovingArea(_currentUnit.transform.position, _currentUnit.Range);
            }
        }

        private void OnStartMovingHandler()
        {
            _particleSystem.Clear();
        }

        public void HighlightMovingArea(Vector3 center, float range)
        {
            _lastUpdateTime = Time.realtimeSinceStartup;

            _particleSystem.Clear();

            var points = GetArea(center, range);

            var particles = new ParticleSystem.Particle[points.Count];
            Color color;
            for (int i = 0; i < points.Count; i++)
            {
                particles[i].position = points[i].Position;
                particles[i].startSize = 1.5f;
                color = Color.Lerp(Color.green, Color.red, points[i].Distance);
                color.a = 0.2f;
                particles[i].startColor = color;
                particles[i].remainingLifetime = 100f;
                particles[i].startLifetime = 100f;
            }

            _currentNumberOfParticles = particles.Length;

            _particleSystem.SetParticles(particles, particles.Length);
            _particleSystem.Play();
        }


        public List<AreaPointData> GetArea(Vector3 center, float range)
        {
            List<AreaPointData> result = new List<AreaPointData>();

            int cellsRange = Mathf.RoundToInt(range / _cellSize);

            Vector3 startPoint = center - Vector3.one * (cellsRange * _cellSize);
            startPoint.y = center.y;
            Vector3 currentPoint = startPoint;
            NavMeshPath _currentPath = new NavMeshPath();

            _currentNumberOfNavMeshPoints = (cellsRange * 2) * (cellsRange * 2);

            for (int x = 0; x < cellsRange * 2; ++x)
            {
                for (int z = 0; z < cellsRange * 2; ++z)
                {
                    currentPoint.x = startPoint.x + (x * _cellSize);
                    currentPoint.z = startPoint.z + (z * _cellSize);
                    if (Vector3.Distance(currentPoint, center) >= _cellSize)
                    {
                        if (Vector3.Distance(currentPoint, center) <= range)
                        {
                            NavMeshHit hit;
                            if (NavMesh.Raycast(center, currentPoint, out hit, 1))
                            {
                                if (NavMesh.CalculatePath(center, currentPoint, 1, _currentPath))
                                {
                                    var corners = _currentPath.corners;
                                    if (corners.Length > 2)
                                    {
                                        bool tooFar = false;
                                        float distance = 0;
                                        for (int i = 1; i < corners.Length; ++i)
                                        {
                                            distance += Vector3.Distance(corners[i], corners[i - 1]);
                                            if (distance > range)
                                            {
                                                // too far
                                                tooFar = true;
                                                break;
                                            }
                                        }
                                        if (tooFar == false)
                                        {
                                            result.Add(new AreaPointData
                                            {
                                                Distance = Vector3.Distance(currentPoint, center) / range,
                                                Position = currentPoint
                                            });
                                        }
                                    }
                                    else
                                    {
                                        // Straight path
                                        result.Add(new AreaPointData
                                        {
                                            Distance = Vector3.Distance(currentPoint, center) / range,
                                            Position = currentPoint
                                        });
                                    }
                                }
                                else
                                {
                                    //unreachable
                                }
                            }
                            else
                            {
                                // straight Path
                                result.Add(new AreaPointData
                                {
                                    Distance = Vector3.Distance(currentPoint, center) / range,
                                    Position = currentPoint
                                });
                            }
                        }
                        else
                        {
                            //Out of range
                        }
                    }
                    else
                    {
                        // it`s in center!
                    }
                }
            }
            return result;
        }
    }
}
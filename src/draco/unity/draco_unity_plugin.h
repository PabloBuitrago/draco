// Copyright 2017 The Draco Authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#ifndef DRACO_UNITY_DRACO_UNITY_PLUGIN_H_
#define DRACO_UNITY_DRACO_UNITY_PLUGIN_H_

#include "draco/compression/config/compression_shared.h"
#include "draco/compression/decode.h"

#ifdef BUILD_UNITY_PLUGIN

#if _MSC_VER // this is defined when compiling with Visual Studio
#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this
#else
#define EXPORT_API // XCode does not need annotating exported functions, so define is empty
#endif

namespace draco {

extern "C" {
  struct EXPORT_API DracoToUnityPointCloud {

    int num_vertices;
    float *position;
    float *color;
    int num_points;
    int *vertex_indices;

  };

  int EXPORT_API DecodePointCloudForUnity(char *data, unsigned int length,
                         DracoToUnityPointCloud **tmp_point_cloud);
}  // extern "C"

}  // namespace draco

#endif  // BUILD_UNITY_PLUGIN

#endif  // DRACO_UNITY_DRACO_UNITY_PLUGIN_H_

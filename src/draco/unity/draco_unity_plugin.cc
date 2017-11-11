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
#include "draco/unity/draco_unity_plugin.h"

#ifdef BUILD_UNITY_PLUGIN

namespace draco {

int DecodePointCloudForUnity(char *data, unsigned int length,
                       DracoToUnityPointCloud **tmp_point_cloud) {
  draco::DecoderBuffer buffer;
  buffer.Init(data, length);
  auto type_statusor = draco::Decoder::GetEncodedGeometryType(&buffer);
  if (!type_statusor.ok()) {
    return -1;
  }
  const draco::EncodedGeometryType geom_type = type_statusor.value();
  if (geom_type != draco::POINT_CLOUD) {
    return -2;
  }

  draco::Decoder decoder;
  auto statusor = decoder.DecodePointCloudFromBuffer(&buffer);
  if (!statusor.ok()) {
    return -3;
  }
  std::unique_ptr<draco::PointCloud> in_point_cloud = std::move(statusor).value();

  *tmp_point_cloud = new DracoToUnityPointCloud();
  DracoToUnityPointCloud *unity_point_cloud = *tmp_point_cloud;
  unity_point_cloud->num_points = in_point_cloud->num_points();
  unity_point_cloud->num_vertices = in_point_cloud->num_points();
  
  
  // std::unique_ptr<draco::PointCloud> pc;
  // pc = std::move(in_point_cloud);
  unity_point_cloud->position = new float[in_point_cloud->num_points() * 3];
  const auto pos_att =
      in_point_cloud->GetNamedAttribute(draco::GeometryAttribute::POSITION);
  std::array<float, 3> pos_value;
  for (draco::PointIndex i(0); i < in_point_cloud->num_points(); ++i) {
    const draco::AttributeValueIndex val_index = pos_att->mapped_index(i);
    if (!pos_att->ConvertValue<float, 3>(val_index, &pos_value[0])) return -8;
    memcpy(unity_point_cloud->position + i.value() * 3, pos_value.data(),
           sizeof(float) * 3);
  }
    
    unity_point_cloud->color = new float[in_point_cloud->num_points() * 4];
    const auto col_att =
    in_point_cloud->GetNamedAttribute(draco::GeometryAttribute::COLOR);
    std::array<float, 4> col_value;
    for (draco::PointIndex i(0); i < in_point_cloud->num_points(); ++i) {
        const draco::AttributeValueIndex val_index = col_att->mapped_index(i);
        if (!col_att->ConvertValue<float, 4>(val_index, &col_value[0])) return -8;
        memcpy(unity_point_cloud->color + i.value() * 4, col_value.data(),
               sizeof(float) * 4);
    }

  return in_point_cloud->num_points();
}
    
}  // namespace draco

#endif // BUILD_UNITY_PLUGIN

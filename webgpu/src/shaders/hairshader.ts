export function hair_shader() {
    return {
        label: "hair shader",
        code: `
          @group(0) @binding(0) var<uniform> xTransform: mat4x4<f32>;
          @group(0) @binding(1) var<uniform> screen: vec2f;
          @group(0) @binding(2) var<storage> signals: array<SignalAttributes>;
  
          struct SignalAttributes{
            color : vec4f,
            // structs must have a size equal to a multiple of 16
            // the @size(16) is actually only relevant if there is another variable
            @size(16)  thickness : f32,
            transform : mat4x4<f32>,
          }
  
          struct VertexOut{
            @builtin(position) position : vec4f,
            @location(0)@interpolate(flat) index : i32,
          }
  
          @vertex
          fn vertexMain(@location(0) position: vec2f, @location(1) signedIndex: vec2f)
            -> VertexOut {     
            // dummy so auto layout sees the uniform screen variable                        
            let dummy = screen;
            let index = i32(signedIndex.x);
            let x = xTransform * signals[index].transform*vec4f(position,0,1);                  
            return VertexOut(x, index);          
          }         
  
          @fragment
          fn fragmentMain(in: VertexOut) -> @location(0) vec4f {
            return signals[in.index].color;
          }
        `
    };
}
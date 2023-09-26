export function fastline_shader(useAntiAliasing: boolean) {
  return {
    label: "line shader",
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
            // distance of the vertex from the line in screen space 
            ${useAntiAliasing?'':'//'}@location(1)@interpolate(linear) screenDist : vec2f,
          }
  
          // id holds two informations:
          // id.x corresponds to the index of the signal and has to be cast to an int32
          // id.y has the value 1,2,3 or 4 corresponding to the vertex positions left bottom, left top, right bottom and right top          
          @vertex
          fn vertexMain(@location(0) A: vec2f, @location(1) B: vec2f, @location(2) C: vec2f, @location(3) id: vec2f)
            -> VertexOut {                             
            
            let index = i32(id.x);
            let signal = signals[index];
            let thickness = signal.thickness ${useAntiAliasing ? '+ 3.0' : ''};
            let radius = thickness/2.0;
            // aspect ratio correction
            let uscreen = vec2f(1, screen.y/screen.x);       
            // u for uniform, meaning -1 to 1, this also explain
            let uradius = thickness/screen.x;
            let dir = select(1.0,-1.0, id.y==1 || id.y==3);
  
            let transform = xTransform * signal.transform;
            var a = (transform*vec4f(A, 0, 1)).xy;
            let b = (transform*vec4f(B, 0, 1)).xy;
            var c = (transform*vec4f(C, 0, 1)).xy;
            // correcting if two points are identical
            a = select(a, b + normalize(b-c), length(b-a)==0);
            c = select(c, b + normalize(b-a), length(b-c)==0);
            let ab = normalize(normalize((b-a))*uscreen);
            let bc = normalize(normalize((c-b))*uscreen);
            let normalA = vec2f(-ab.y,ab.x);
            let normalC = vec2f(-bc.y,bc.x);

            // this would give a line with a miter instead of bevel
            // but it has to be beveled anyways if the meter gets too long
            // as long as this isn't implemented you can't really use this section
            //let tangent = normalize(ab+bc);
            //let miter = vec2f(-tangent.y,tangent.x);
            //let miterLength = 1 / dot(miter, normalA); 
            //let pos = b+dir*uradius*miter*miterLength/uscreen;

            let normal = select(normalA, normalC, id.y==3 || id.y==4);
            let pos = b+dir*uradius*normal/uscreen; 
            ${useAntiAliasing?'//':''} return VertexOut(vec4f(pos , 0, 1), index);
           
            // distance of vertex to the line in pixelspace/screenspace
            // fragment shader is rendered at the center of each pixel
            // the fragment shader receives an interpolated value as the center of the pixel doesn't align with a vertex
            ${useAntiAliasing?'':'//'} let tmp = select(1.0, -1.0, id.y==2 || id.y==4);
            ${useAntiAliasing?'':'//'} let screenDist = vec2f(tmp*radius,0);
            ${useAntiAliasing?'':'//'} return VertexOut(vec4f(pos , 0, 1), index, screenDist);
          }         
  
          ${useAntiAliasing ? aaFragment : simpleFragment}
        `
  };
}

const simpleFragment = `
  @fragment
  fn fragmentMain(in: VertexOut) -> @location(0) vec4f {
    return signals[in.index].color;
  }
  `

const aaFragment = `
@fragment
fn fragmentMain(in: VertexOut) -> @location(0) vec4f {
  let radius = signals[in.index].thickness/2.0;
  let smoothSize = 0.5;
  var x = abs(in.screenDist.x);
  var a = 1-smoothstep(radius - smoothSize, radius + smoothSize, x);                                     
  return vec4f(signals[in.index].color.xyz, a);
}
`
